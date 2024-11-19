// Protocol Buffers - Google's data interchange format
// Copyright 2023 Google LLC.  All rights reserved.
//
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file or at
// https://developers.google.com/open-source/licenses/bsd

#include "upb_generator/protoc-gen-upb_minitable.h"

#include <string.h>

#include <algorithm>
#include <cstddef>
#include <cstdint>
#include <cstdio>
#include <cstdlib>
#include <map>
#include <string>
#include <utility>
#include <vector>

#include "absl/container/flat_hash_set.h"
#include "absl/log/absl_check.h"
#include "absl/strings/str_cat.h"
#include "absl/strings/string_view.h"
#include "absl/strings/substitute.h"
#include "upb/base/descriptor_constants.h"
#include "upb/mini_table/enum.h"
#include "upb/mini_table/field.h"
#include "upb/mini_table/internal/field.h"
#include "upb/mini_table/message.h"
#include "upb/reflection/def.hpp"
#include "upb/wire/types.h"
#include "upb_generator/common.h"
#include "upb_generator/file_layout.h"

// Must be last.
#include "upb/port/def.inc"
#include "upb_generator/plugin.h"

namespace upb {
namespace generator {
namespace {

// Returns fields in order of "hotness", eg. how frequently they appear in
// serialized payloads. Ideally this will use a profile. When we don't have
// that, we assume that fields with smaller numbers are used more frequently.
inline std::vector<upb::FieldDefPtr> FieldHotnessOrder(
    upb::MessageDefPtr message) {
  std::vector<upb::FieldDefPtr> fields;
  size_t field_count = message.field_count();
  fields.reserve(field_count);
  for (size_t i = 0; i < field_count; i++) {
    fields.push_back(message.field(i));
  }
  std::sort(fields.begin(), fields.end(),
            [](upb::FieldDefPtr a, upb::FieldDefPtr b) {
              return std::make_pair(!a.is_required(), a.number()) <
                     std::make_pair(!b.is_required(), b.number());
            });
  return fields;
}

std::string ExtensionIdentBase(upb::FieldDefPtr ext) {
  UPB_ASSERT(ext.is_extension());
  std::string ext_scope;
  if (ext.extension_scope()) {
    return MessageName(ext.extension_scope());
  } else {
    return ToCIdent(ext.file().package());
  }
}

std::string ExtensionLayout(upb::FieldDefPtr ext) {
  return absl::StrCat(ExtensionIdentBase(ext), "_", ext.name(), "_ext");
}

std::string MessagePtrName(upb::MessageDefPtr message) {
  return MessageInitName(message) + "_ptr";
}

const char* kEnumsInit = "enums_layout";
const char* kExtensionsInit = "extensions_layout";
const char* kMessagesInit = "messages_layout";

typedef std::pair<std::string, uint64_t> TableEntry;

uint32_t GetWireTypeForField(upb::FieldDefPtr field) {
  if (field.packed()) return kUpb_WireType_Delimited;
  switch (field.type()) {
    case kUpb_FieldType_Double:
    case kUpb_FieldType_Fixed64:
    case kUpb_FieldType_SFixed64:
      return kUpb_WireType_64Bit;
    case kUpb_FieldType_Float:
    case kUpb_FieldType_Fixed32:
    case kUpb_FieldType_SFixed32:
      return kUpb_WireType_32Bit;
    case kUpb_FieldType_Int64:
    case kUpb_FieldType_UInt64:
    case kUpb_FieldType_Int32:
    case kUpb_FieldType_Bool:
    case kUpb_FieldType_UInt32:
    case kUpb_FieldType_Enum:
    case kUpb_FieldType_SInt32:
    case kUpb_FieldType_SInt64:
      return kUpb_WireType_Varint;
    case kUpb_FieldType_Group:
      return kUpb_WireType_StartGroup;
    case kUpb_FieldType_Message:
    case kUpb_FieldType_String:
    case kUpb_FieldType_Bytes:
      return kUpb_WireType_Delimited;
  }
  UPB_UNREACHABLE();
}

uint32_t MakeTag(uint32_t field_number, uint32_t wire_type) {
  return field_number << 3 | wire_type;
}

size_t WriteVarint32ToArray(uint64_t val, char* buf) {
  size_t i = 0;
  do {
    uint8_t byte = val & 0x7fU;
    val >>= 7;
    if (val) byte |= 0x80U;
    buf[i++] = byte;
  } while (val);
  return i;
}

uint64_t GetEncodedTag(upb::FieldDefPtr field) {
  uint32_t wire_type = GetWireTypeForField(field);
  uint32_t unencoded_tag = MakeTag(field.number(), wire_type);
  char tag_bytes[10] = {0};
  WriteVarint32ToArray(unencoded_tag, tag_bytes);
  uint64_t encoded_tag = 0;
  memcpy(&encoded_tag, tag_bytes, sizeof(encoded_tag));
  // TODO: byte-swap for big endian.
  return encoded_tag;
}

int GetTableSlot(upb::FieldDefPtr field) {
  uint64_t tag = GetEncodedTag(field);
  if (tag > 0x7fff) {
    // Tag must fit within a two-byte varint.
    return -1;
  }
  return (tag & 0xf8) >> 3;
}

bool TryFillTableEntry(const DefPoolPair& pools, upb::FieldDefPtr field,
                       TableEntry& ent) {
  const upb_MiniTable* mt = pools.GetMiniTable64(field.containing_type());
  const upb_MiniTableField* mt_f =
      upb_MiniTable_FindFieldByNumber(mt, field.number());
  std::string type = "";
  std::string cardinality = "";
  switch (upb_MiniTableField_Type(mt_f)) {
    case kUpb_FieldType_Bool:
      type = "b1";
      break;
    case kUpb_FieldType_Enum:
      if (upb_MiniTableField_IsClosedEnum(mt_f)) {
        // We don't have the means to test proto2 enum fields for valid values.
        return false;
      }
      [[fallthrough]];
    case kUpb_FieldType_Int32:
    case kUpb_FieldType_UInt32:
      type = "v4";
      break;
    case kUpb_FieldType_Int64:
    case kUpb_FieldType_UInt64:
      type = "v8";
      break;
    case kUpb_FieldType_Fixed32:
    case kUpb_FieldType_SFixed32:
    case kUpb_FieldType_Float:
      type = "f4";
      break;
    case kUpb_FieldType_Fixed64:
    case kUpb_FieldType_SFixed64:
    case kUpb_FieldType_Double:
      type = "f8";
      break;
    case kUpb_FieldType_SInt32:
      type = "z4";
      break;
    case kUpb_FieldType_SInt64:
      type = "z8";
      break;
    case kUpb_FieldType_String:
      type = "s";
      break;
    case kUpb_FieldType_Bytes:
      type = "b";
      break;
    case kUpb_FieldType_Message:
      type = "m";
      break;
    default:
      return false;  // Not supported yet.
  }

  if (upb_MiniTableField_IsArray(mt_f)) {
    cardinality = upb_MiniTableField_IsPacked(mt_f) ? "p" : "r";
  } else if (upb_MiniTableField_IsScalar(mt_f)) {
    cardinality = upb_MiniTableField_IsInOneof(mt_f) ? "o" : "s";
  } else {
    return false;  // Not supported yet (ever?).
  }

  uint64_t expected_tag = GetEncodedTag(field);

  // Data is:
  //
  //                  48                32                16                 0
  // |--------|--------|--------|--------|--------|--------|--------|--------|
  // |   offset (16)   |case offset (16) |presence| submsg |  exp. tag (16)  |
  // |--------|--------|--------|--------|--------|--------|--------|--------|
  //
  // - |presence| is either hasbit index or field number for oneofs.

  uint64_t data =
      static_cast<uint64_t>(mt_f->UPB_PRIVATE(offset)) << 48 | expected_tag;

  if (field.IsSequence()) {
    // No hasbit/oneof-related fields.
  }
  if (field.real_containing_oneof()) {
    uint64_t case_offset = ~mt_f->presence;
    if (case_offset > 0xffff || field.number() > 0xff) return false;
    data |= field.number() << 24;
    data |= case_offset << 32;
  } else {
    uint64_t hasbit_index = 63;  // No hasbit (set a high, unused bit).
    if (mt_f->presence) {
      hasbit_index = mt_f->presence;
      if (hasbit_index > 31) return false;
    }
    data |= hasbit_index << 24;
  }

  if (field.ctype() == kUpb_CType_Message) {
    uint64_t idx = mt_f->UPB_PRIVATE(submsg_index);
    if (idx > 255) return false;
    data |= idx << 16;

    std::string size_ceil = "max";
    size_t size = SIZE_MAX;
    if (field.message_type().file() == field.file()) {
      // We can only be guaranteed the size of the sub-message if it is in the
      // same file as us.  We could relax this to increase the speed of
      // cross-file sub-message parsing if we are comfortable requiring that
      // users compile all messages at the same time.
      const upb_MiniTable* sub_mt = pools.GetMiniTable64(field.message_type());
      size = sub_mt->UPB_PRIVATE(size) + 8;
    }
    std::vector<size_t> breaks = {64, 128, 192, 256};
    for (auto brk : breaks) {
      if (size <= brk) {
        size_ceil = std::to_string(brk);
        break;
      }
    }
    ent.first = absl::Substitute("upb_p$0$1_$2bt_max$3b", cardinality, type,
                                 expected_tag > 0xff ? "2" : "1", size_ceil);

  } else {
    ent.first = absl::Substitute("upb_p$0$1_$2bt", cardinality, type,
                                 expected_tag > 0xff ? "2" : "1");
  }
  ent.second = data;
  return true;
}

std::vector<TableEntry> FastDecodeTable(upb::MessageDefPtr message,
                                        const DefPoolPair& pools) {
  std::vector<TableEntry> table;
  for (const auto field : FieldHotnessOrder(message)) {
    TableEntry ent;
    int slot = GetTableSlot(field);
    // std::cerr << "table slot: " << field->number() << ": " << slot << "\n";
    if (slot < 0) {
      // Tag can't fit in the table.
      continue;
    }
    if (!TryFillTableEntry(pools, field, ent)) {
      // Unsupported field type or offset, hasbit index, etc. doesn't fit.
      continue;
    }
    while ((size_t)slot >= table.size()) {
      size_t size = std::max(static_cast<size_t>(1), table.size() * 2);
      table.resize(size, TableEntry{"_upb_FastDecoder_DecodeGeneric", 0});
    }
    if (table[slot].first != "_upb_FastDecoder_DecodeGeneric") {
      // A hotter field already filled this slot.
      continue;
    }
    table[slot] = ent;
  }
  return table;
}

std::string ArchDependentSize(int64_t size32, int64_t size64) {
  if (size32 == size64) return absl::StrCat(size32);
  return absl::Substitute("UPB_SIZE($0, $1)", size32, size64);
}

std::string FieldInitializer(const DefPoolPair& pools, upb::FieldDefPtr field) {
  return upb::generator::FieldInitializer(field, pools.GetField64(field),
                                          pools.GetField32(field));
}

// Writes a single field into a .upb.c source file.
void WriteMessageField(upb::FieldDefPtr field,
                       const upb_MiniTableField* field64,
                       const upb_MiniTableField* field32, Output& output) {
  output("  $0,\n", upb::generator::FieldInitializer(field, field64, field32));
}

std::string GetSub(upb::FieldDefPtr field, bool is_extension) {
  if (auto message_def = field.message_type()) {
    return absl::Substitute("{.UPB_PRIVATE(submsg) = &$0}",
                            is_extension ? MessageInitName(message_def)
                                         : MessagePtrName(message_def));
  }

  if (auto enum_def = field.enum_subdef()) {
    if (enum_def.is_closed()) {
      return absl::Substitute("{.UPB_PRIVATE(subenum) = &$0}",
                              EnumInit(enum_def));
    }
  }

  return std::string("{.UPB_PRIVATE(submsg) = NULL}");
}

bool IsCrossFile(upb::FieldDefPtr field) {
  return field.message_type() != field.containing_type();
}

// Writes a single message into a .upb.c source file.
void WriteMessage(upb::MessageDefPtr message, const DefPoolPair& pools,
                  const MiniTableOptions& options, Output& output) {
  std::string msg_name = ToCIdent(message.full_name());
  std::string fields_array_ref = "NULL";
  std::string submsgs_array_ref = "NULL";
  std::string subenums_array_ref = "NULL";
  const upb_MiniTable* mt_32 = pools.GetMiniTable32(message);
  const upb_MiniTable* mt_64 = pools.GetMiniTable64(message);
  std::map<int, std::string> subs;
  absl::flat_hash_set<const upb_MiniTable*> seen;

  // Construct map of sub messages by field number.
  for (int i = 0; i < mt_64->UPB_PRIVATE(field_count); i++) {
    const upb_MiniTableField* f = &mt_64->UPB_PRIVATE(fields)[i];
    uint32_t index = f->UPB_PRIVATE(submsg_index);
    if (index != kUpb_NoSub) {
      const int f_number = upb_MiniTableField_Number(f);
      upb::FieldDefPtr field = message.FindFieldByNumber(f_number);
      auto pair = subs.emplace(index, GetSub(field, false));
      ABSL_CHECK(pair.second);
      if (options.one_output_per_message && field.IsSubMessage() &&
          IsCrossFile(field) && !upb_MiniTableField_IsMap(f)) {
        if (seen.insert(pools.GetMiniTable64(field.message_type())).second) {
          output(
              "__attribute__((weak)) const upb_MiniTable* $0 ="
              " &UPB_PRIVATE(_kUpb_MiniTable_StaticallyTreeShaken);\n",
              MessagePtrName(field.message_type()));
        }
      }
    }
  }
  // Write upb_MiniTableSubInternal table for sub messages referenced from
  // fields.
  if (!subs.empty()) {
    std::string submsgs_array_name = msg_name + "_submsgs";
    submsgs_array_ref = "&" + submsgs_array_name + "[0]";
    output("static const upb_MiniTableSubInternal $0[$1] = {\n",
           submsgs_array_name, subs.size());

    int i = 0;
    for (const auto& pair : subs) {
      ABSL_CHECK(pair.first == i++);
      output("  $0,\n", pair.second);
    }

    output("};\n\n");
  }

  // Write upb_MiniTableField table.
  if (mt_64->UPB_PRIVATE(field_count) > 0) {
    std::string fields_array_name = msg_name + "__fields";
    fields_array_ref = "&" + fields_array_name + "[0]";
    output("static const upb_MiniTableField $0[$1] = {\n", fields_array_name,
           mt_64->UPB_PRIVATE(field_count));
    for (int i = 0; i < mt_64->UPB_PRIVATE(field_count); i++) {
      WriteMessageField(message.FindFieldByNumber(
                            mt_64->UPB_PRIVATE(fields)[i].UPB_PRIVATE(number)),
                        &mt_64->UPB_PRIVATE(fields)[i],
                        &mt_32->UPB_PRIVATE(fields)[i], output);
    }
    output("};\n\n");
  }

  std::vector<TableEntry> table;
  uint8_t table_mask = ~0;

  table = FastDecodeTable(message, pools);

  if (table.size() > 1) {
    UPB_ASSERT((table.size() & (table.size() - 1)) == 0);
    table_mask = (table.size() - 1) << 3;
  }

  std::string msgext = "kUpb_ExtMode_NonExtendable";

  if (message.extension_range_count()) {
    if (UPB_DESC(MessageOptions_message_set_wire_format)(message.options())) {
      msgext = "kUpb_ExtMode_IsMessageSet";
    } else {
      msgext = "kUpb_ExtMode_Extendable";
    }
  }

  output("const upb_MiniTable $0 = {\n", MessageInitName(message));
  output("  $0,\n", submsgs_array_ref);
  output("  $0,\n", fields_array_ref);
  output("  $0, $1, $2, $3, UPB_FASTTABLE_MASK($4), $5,\n",
         ArchDependentSize(mt_32->UPB_PRIVATE(size), mt_64->UPB_PRIVATE(size)),
         mt_64->UPB_PRIVATE(field_count), msgext,
         mt_64->UPB_PRIVATE(dense_below), table_mask,
         mt_64->UPB_PRIVATE(required_count));
  output("#ifdef UPB_TRACING_ENABLED\n");
  output("  \"$0\",\n", message.full_name());
  output("#endif\n");
  if (!table.empty()) {
    output("  UPB_FASTTABLE_INIT({\n");
    for (const auto& ent : table) {
      output("    {0x$1, &$0},\n", ent.first,
             absl::StrCat(absl::Hex(ent.second, absl::kZeroPad16)));
    }
    output("  })\n");
  }
  output("};\n\n");
  output("const upb_MiniTable* $0 = &$1;\n", MessagePtrName(message),
         MessageInitName(message));
}

void WriteEnum(upb::EnumDefPtr e, Output& output) {
  std::string values_init = "{\n";
  const upb_MiniTableEnum* mt = e.mini_table();
  uint32_t value_count =
      (mt->UPB_PRIVATE(mask_limit) / 32) + mt->UPB_PRIVATE(value_count);
  for (uint32_t i = 0; i < value_count; i++) {
    absl::StrAppend(&values_init, "                0x",
                    absl::Hex(mt->UPB_PRIVATE(data)[i]), ",\n");
  }
  values_init += "    }";

  output(
      R"cc(
        const upb_MiniTableEnum $0 = {
            $1,
            $2,
            $3,
        };
      )cc",
      EnumInit(e), mt->UPB_PRIVATE(mask_limit), mt->UPB_PRIVATE(value_count),
      values_init);
  output("\n");
}

void WriteExtension(const DefPoolPair& pools, upb::FieldDefPtr ext,
                    Output& output) {
  output("UPB_LINKARR_APPEND(upb_AllExts)\n");
  output("const upb_MiniTableExtension $0 = {\n  ", ExtensionLayout(ext));
  output("$0,\n", FieldInitializer(pools, ext));
  output("  &$0,\n", MessageInitName(ext.containing_type()));
  output("  $0,\n", GetSub(ext, true));
  output("\n};\n");
}

}  // namespace

void WriteMiniTableHeader(const DefPoolPair& pools, upb::FileDefPtr file,
                          Output& output) {
  EmitFileWarning(file.name(), output);
  output(
      "#ifndef $0_UPB_MINITABLE_H_\n"
      "#define $0_UPB_MINITABLE_H_\n\n"
      "#include \"upb/generated_code_support.h\"\n",
      ToPreproc(file.name()));

  for (int i = 0; i < file.public_dependency_count(); i++) {
    if (i == 0) {
      output("/* Public Imports. */\n");
    }
    output("#include \"$0\"\n",
           MiniTableHeaderFilename(file.public_dependency(i)));
    if (i == file.public_dependency_count() - 1) {
      output("\n");
    }
  }

  output(
      "\n"
      "// Must be last.\n"
      "#include \"upb/port/def.inc\"\n"
      "\n"
      "#ifdef __cplusplus\n"
      "extern \"C\" {\n"
      "#endif\n"
      "\n");

  const std::vector<upb::MessageDefPtr> this_file_messages =
      SortedMessages(file);
  const std::vector<upb::FieldDefPtr> this_file_exts = SortedExtensions(file);

  for (auto message : this_file_messages) {
    output("extern const upb_MiniTable $0;\n", MessageInitName(message));
    output("extern const upb_MiniTable* $0;\n", MessagePtrName(message));
  }
  for (auto ext : this_file_exts) {
    output("extern const upb_MiniTableExtension $0;\n", ExtensionLayout(ext));
  }

  output("\n");

  std::vector<upb::EnumDefPtr> this_file_enums =
      SortedEnums(file, kClosedEnums);

  for (const auto enumdesc : this_file_enums) {
    output("extern const upb_MiniTableEnum $0;\n", EnumInit(enumdesc));
  }

  output("extern const upb_MiniTableFile $0;\n\n", FileLayoutName(file));

  output(
      "#ifdef __cplusplus\n"
      "}  /* extern \"C\" */\n"
      "#endif\n"
      "\n"
      "#include \"upb/port/undef.inc\"\n"
      "\n"
      "#endif  /* $0_UPB_MINITABLE_H_ */\n",
      ToPreproc(file.name()));
}

void WriteMiniTableSourceIncludes(upb::FileDefPtr file, Output& output) {
  EmitFileWarning(file.name(), output);

  output(
      "#include <stddef.h>\n"
      "#include \"upb/generated_code_support.h\"\n"
      "#include \"$0\"\n",
      MiniTableHeaderFilename(file));

  for (int i = 0; i < file.dependency_count(); i++) {
    output("#include \"$0\"\n", MiniTableHeaderFilename(file.dependency(i)));
  }

  output(
      "\n"
      "// Must be last.\n"
      "#include \"upb/port/def.inc\"\n"
      "\n");

  output(
      "extern const struct upb_MiniTable "
      "UPB_PRIVATE(_kUpb_MiniTable_StaticallyTreeShaken);\n");
}

void WriteMiniTableSource(const DefPoolPair& pools, upb::FileDefPtr file,
                          const MiniTableOptions& options, Output& output) {
  WriteMiniTableSourceIncludes(file, output);

  std::vector<upb::MessageDefPtr> messages = SortedMessages(file);
  std::vector<upb::FieldDefPtr> extensions = SortedExtensions(file);
  std::vector<upb::EnumDefPtr> enums = SortedEnums(file, kClosedEnums);

  if (options.one_output_per_message) {
    for (auto message : messages) {
      output("extern const upb_MiniTable* $0;\n", MessagePtrName(message));
    }
    for (const auto e : enums) {
      output("extern const upb_MiniTableEnum $0;\n", EnumInit(e));
    }
    for (const auto ext : extensions) {
      output("extern const upb_MiniTableExtension $0;\n", ExtensionLayout(ext));
    }
  } else {
    for (auto message : messages) {
      WriteMessage(message, pools, options, output);
    }
    for (const auto e : enums) {
      WriteEnum(e, output);
    }
    for (const auto ext : extensions) {
      WriteExtension(pools, ext, output);
    }
  }

  // Messages.
  if (!messages.empty()) {
    output("static const upb_MiniTable *$0[$1] = {\n", kMessagesInit,
           messages.size());
    for (auto message : messages) {
      output("  &$0,\n", MessageInitName(message));
    }
    output("};\n");
    output("\n");
  }

  // Enums.
  if (!enums.empty()) {
    output("static const upb_MiniTableEnum *$0[$1] = {\n", kEnumsInit,
           enums.size());
    for (const auto e : enums) {
      output("  &$0,\n", EnumInit(e));
    }
    output("};\n");
    output("\n");
  }

  if (!extensions.empty()) {
    // Extensions.
    output(
        "\n"
        "static const upb_MiniTableExtension *$0[$1] = {\n",
        kExtensionsInit, extensions.size());

    for (auto ext : extensions) {
      output("  &$0,\n", ExtensionLayout(ext));
    }

    output(
        "};\n"
        "\n");
  }

  output("const upb_MiniTableFile $0 = {\n", FileLayoutName(file));
  output("  $0,\n", messages.empty() ? "NULL" : kMessagesInit);
  output("  $0,\n", enums.empty() ? "NULL" : kEnumsInit);
  output("  $0,\n", extensions.empty() ? "NULL" : kExtensionsInit);
  output("  $0,\n", messages.size());
  output("  $0,\n", enums.size());
  output("  $0,\n", extensions.size());
  output("};\n\n");

  output("#include \"upb/port/undef.inc\"\n");
  output("\n");
}

std::string MultipleSourceFilename(upb::FileDefPtr file,
                                   absl::string_view full_name, int* i) {
  *i += 1;
  return absl::StrCat(StripExtension(file.name()), ".upb_weak_minitables/",
                      *i, ".upb.c");
}

void WriteMiniTableMultipleSources(const DefPoolPair& pools,
                                   upb::FileDefPtr file,
                                   const MiniTableOptions& options,
                                   Plugin* plugin) {
  std::vector<upb::MessageDefPtr> messages = SortedMessages(file);
  std::vector<upb::FieldDefPtr> extensions = SortedExtensions(file);
  std::vector<upb::EnumDefPtr> enums = SortedEnums(file, kClosedEnums);
  int i = 0;

  for (auto message : messages) {
    Output output;
    WriteMiniTableSourceIncludes(file, output);
    WriteMessage(message, pools, options, output);
    plugin->AddOutputFile(MultipleSourceFilename(file, message.full_name(), &i),
                          output.output());
  }
  for (const auto e : enums) {
    Output output;
    WriteMiniTableSourceIncludes(file, output);
    WriteEnum(e, output);
    plugin->AddOutputFile(MultipleSourceFilename(file, e.full_name(), &i),
                          output.output());
  }
  for (const auto ext : extensions) {
    Output output;
    WriteMiniTableSourceIncludes(file, output);
    WriteExtension(pools, ext, output);
    plugin->AddOutputFile(MultipleSourceFilename(file, ext.full_name(), &i),
                          output.output());
  }
}

}  // namespace generator
}  // namespace upb
