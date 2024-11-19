// Protocol Buffers - Google's data interchange format
// Copyright 2023 Google LLC.  All rights reserved.
//
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file or at
// https://developers.google.com/open-source/licenses/bsd

#include <memory>

#include "google/protobuf/descriptor.upb.h"
#include "upb/reflection/def.hpp"
#include "upb/util/def_to_proto.h"
#include "upb_generator/common.h"
#include "upb_generator/file_layout.h"
#include "upb_generator/plugin.h"

namespace upb {
namespace generator {
namespace {

struct Options {
  std::string dllexport_decl;
};

std::string DefInitSymbol(upb::FileDefPtr file) {
  return ToCIdent(file.name()) + "_upbdefinit";
}

static std::string DefHeaderFilename(upb::FileDefPtr file) {
  return StripExtension(file.name()) + ".upbdefs.h";
}

static std::string DefSourceFilename(upb::FileDefPtr file) {
  return StripExtension(file.name()) + ".upbdefs.c";
}

void GenerateMessageDefAccessor(upb::MessageDefPtr d, Output& output) {
  output("UPB_INLINE const upb_MessageDef *$0_getmsgdef(upb_DefPool *s) {\n",
         ToCIdent(d.full_name()));
  output("  _upb_DefPool_LoadDefInit(s, &$0);\n", DefInitSymbol(d.file()));
  output("  return upb_DefPool_FindMessageByName(s, \"$0\");\n", d.full_name());
  output("}\n");
  output("\n");
}

void WriteDefHeader(upb::FileDefPtr file, const Options& options,
                    Output& output) {
  EmitFileWarning(file.name(), output);

  output(
      "#ifndef $0_UPBDEFS_H_\n"
      "#define $0_UPBDEFS_H_\n\n"
      "#include \"upb/reflection/def.h\"\n"
      "#include \"upb/reflection/internal/def_pool.h\"\n"
      "\n"
      "#include \"upb/port/def.inc\" // Must be last.\n"
      "#ifdef __cplusplus\n"
      "extern \"C\" {\n"
      "#endif\n\n",
      ToPreproc(file.name()));

  output("extern$1 _upb_DefPool_Init $0;\n", DefInitSymbol(file),
         PadPrefix(options.dllexport_decl));
  output("\n");

  for (auto msg : SortedMessages(file)) {
    GenerateMessageDefAccessor(msg, output);
  }

  output(
      "#ifdef __cplusplus\n"
      "}  /* extern \"C\" */\n"
      "#endif\n"
      "\n"
      "#include \"upb/port/undef.inc\"\n"
      "\n"
      "#endif  /* $0_UPBDEFS_H_ */\n",
      ToPreproc(file.name()));
}

void WriteDefSource(upb::FileDefPtr file, const Options& options,
                    Output& output) {
  EmitFileWarning(file.name(), output);

  output("#include \"upb/reflection/def.h\"\n");
  output("#include \"$0\"\n", DefHeaderFilename(file));
  output("#include \"$0\"\n", MiniTableHeaderFilename(file));
  output("\n");

  for (int i = 0; i < file.dependency_count(); i++) {
    output("extern$1 _upb_DefPool_Init $0;\n",
           DefInitSymbol(file.dependency(i)),
           PadPrefix(options.dllexport_decl));
  }

  upb::Arena arena;
  google_protobuf_FileDescriptorProto* file_proto =
      upb_FileDef_ToProto(file.ptr(), arena.ptr());
  size_t serialized_size;
  const char* serialized = google_protobuf_FileDescriptorProto_serialize(
      file_proto, arena.ptr(), &serialized_size);
  absl::string_view file_data(serialized, serialized_size);

  output("static const char descriptor[$0] = {", serialized_size);

  // C90 only guarantees that strings can be up to 509 characters, and some
  // implementations have limits here (for example, MSVC only allows 64k:
  // https://docs.microsoft.com/en-us/cpp/error-messages/compiler-errors-1/fatal-error-c1091.
  // So we always emit an array instead of a string.
  for (size_t i = 0; i < serialized_size;) {
    for (size_t j = 0; j < 25 && i < serialized_size; ++i, ++j) {
      output("'$0', ", absl::CEscape(file_data.substr(i, 1)));
    }
    output("\n");
  }
  output("};\n\n");

  output("static _upb_DefPool_Init *deps[$0] = {\n",
         file.dependency_count() + 1);
  for (int i = 0; i < file.dependency_count(); i++) {
    output("  &$0,\n", DefInitSymbol(file.dependency(i)));
  }
  output("  NULL\n");
  output("};\n");
  output("\n");

  output("_upb_DefPool_Init $0 = {\n", DefInitSymbol(file));
  output("  deps,\n");
  output("  &$0,\n", FileLayoutName(file));
  output("  \"$0\",\n", file.name());
  output("  UPB_STRINGVIEW_INIT(descriptor, $0)\n", file_data.size());
  output("};\n");
}

void GenerateFile(upb::FileDefPtr file, const Options& options,
                  Plugin* plugin) {
  Output h_def_output;
  WriteDefHeader(file, options, h_def_output);
  plugin->AddOutputFile(DefHeaderFilename(file), h_def_output.output());

  Output c_def_output;
  WriteDefSource(file, options, c_def_output);
  plugin->AddOutputFile(DefSourceFilename(file), c_def_output.output());
}

bool ParseOptions(Plugin* plugin, Options* options) {
  for (const auto& pair : ParseGeneratorParameter(plugin->parameter())) {
    if (pair.first == "dllexport_decl") {
      options->dllexport_decl = pair.second;
    } else {
      plugin->SetError(absl::Substitute("Unknown parameter: $0", pair.first));
      return false;
    }
  }

  return true;
}

}  // namespace
}  // namespace generator
}  // namespace upb

int main(int argc, char** argv) {
  upb::generator::Plugin plugin;
  upb::generator::Options options;
  if (!ParseOptions(&plugin, &options)) return 0;
  plugin.GenerateFiles([&](upb::FileDefPtr file) {
    upb::generator::GenerateFile(file, options, &plugin);
  });
  return 0;
}
