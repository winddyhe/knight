// Protocol Buffers - Google's data interchange format
// Copyright 2023 Google LLC.  All rights reserved.
//
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file or at
// https://developers.google.com/open-source/licenses/bsd

#include <string>

#include "absl/strings/string_view.h"
#include "google/protobuf/compiler/cpp/helpers.h"
#include "google/protobuf/compiler/rust/accessors/accessor_case.h"
#include "google/protobuf/compiler/rust/accessors/generator.h"
#include "google/protobuf/compiler/rust/context.h"
#include "google/protobuf/compiler/rust/naming.h"
#include "google/protobuf/descriptor.h"

namespace google {
namespace protobuf {
namespace compiler {
namespace rust {

void SingularString::InMsgImpl(Context& ctx, const FieldDescriptor& field,
                               AccessorCase accessor_case) const {
  std::string field_name = FieldNameWithCollisionAvoidance(field);
  ctx.Emit(
      {
          {"field", RsSafeName(field_name)},
          {"raw_field_name", field_name},
          {"hazzer_thunk", ThunkName(ctx, field, "has")},
          {"getter_thunk", ThunkName(ctx, field, "get")},
          {"setter_thunk", ThunkName(ctx, field, "set")},
          {"clearer_thunk", ThunkName(ctx, field, "clear")},
          {"proxied_type", RsTypePath(ctx, field)},
          io::Printer::Sub("transform_view",
                           [&] {
                             if (field.type() == FieldDescriptor::TYPE_STRING) {
                               ctx.Emit(R"rs(
              // SAFETY: The runtime doesn't require ProtoStr to be UTF-8.
              unsafe { $pb$::ProtoStr::from_utf8_unchecked(view) }
            )rs");
                             } else {
                               ctx.Emit("view");
                             }
                           })
              .WithSuffix(""),  // This lets `$transform_view$,` work.
          {"view_lifetime", ViewLifetime(accessor_case)},
          {"view_self", ViewReceiver(accessor_case)},
          {"getter",
           [&] {
             ctx.Emit(R"rs(
                pub fn $field$($view_self$) -> $pb$::View<$view_lifetime$, $proxied_type$> {
                  let view = unsafe { $getter_thunk$(self.raw_msg()).as_ref() };
                  $transform_view$
                })rs");
           }},
          {"getter_opt",
           [&] {
             if (!field.has_presence()) return;
             ctx.Emit(R"rs(
            pub fn $raw_field_name$_opt($view_self$) -> $pb$::Optional<$pb$::View<$view_lifetime$, $proxied_type$>> {
                $pb$::Optional::new(
                  self.$field$(),
                  self.has_$raw_field_name$()
                )
              }
          )rs");
           }},
          {"setter_impl",
           [&] {
             if (ctx.is_cpp()) {
               ctx.Emit(R"rs(
                let s = val.into_proxied($pbi$::Private);
                unsafe {
                  $setter_thunk$(
                    self.as_mutator_message_ref($pbi$::Private).msg(),
                    s.into_inner($pbi$::Private).into_raw($pbi$::Private)
                  );
                }
              )rs");
             } else {
               ctx.Emit(R"rs(
                let s = val.into_proxied($pbi$::Private);
                let (view, arena) =
                  s.into_inner($pbi$::Private).into_raw_parts($pbi$::Private);

                let mm_ref =
                  self.as_mutator_message_ref($pbi$::Private);
                let parent_arena = mm_ref.arena($pbi$::Private);

                parent_arena.fuse(&arena);

                unsafe {
                  $setter_thunk$(
                    self.as_mutator_message_ref($pbi$::Private).msg(),
                    view
                  );
                }
              )rs");
             }
           }},
          {"setter",
           [&] {
             if (accessor_case == AccessorCase::VIEW) return;
             ctx.Emit({},
                      R"rs(
              pub fn set_$raw_field_name$(&mut self, val: impl $pb$::IntoProxied<$proxied_type$>) {
                $setter_impl$
              }
            )rs");
           }},
          {"hazzer",
           [&] {
             if (!field.has_presence()) return;
             ctx.Emit({}, R"rs(
                pub fn has_$raw_field_name$($view_self$) -> bool {
                  unsafe { $hazzer_thunk$(self.raw_msg()) }
                })rs");
           }},
          {"clearer",
           [&] {
             if (accessor_case == AccessorCase::VIEW) return;
             if (!field.has_presence()) return;
             ctx.Emit({}, R"rs(
                pub fn clear_$raw_field_name$(&mut self) {
                  unsafe { $clearer_thunk$(self.raw_msg()) }
                })rs");
           }},
      },
      R"rs(
        $getter$
        $getter_opt$
        $setter$
        $hazzer$
        $clearer$
      )rs");
}

void SingularString::InExternC(Context& ctx,
                               const FieldDescriptor& field) const {
  ctx.Emit({{"hazzer_thunk", ThunkName(ctx, field, "has")},
            {"getter_thunk", ThunkName(ctx, field, "get")},
            {"setter_thunk", ThunkName(ctx, field, "set")},
            {"setter",
             [&] {
               if (ctx.is_cpp()) {
                 ctx.Emit(R"rs(
                  fn $setter_thunk$(raw_msg: $pbr$::RawMessage, val: $pbr$::CppStdString);
                )rs");
               } else {
                 ctx.Emit(R"rs(
                  fn $setter_thunk$(raw_msg: $pbr$::RawMessage, val: $pbr$::PtrAndLen);
                )rs");
               }
             }},
            {"clearer_thunk", ThunkName(ctx, field, "clear")},
            {"with_presence_fields_thunks",
             [&] {
               if (field.has_presence()) {
                 ctx.Emit(R"rs(
                     fn $hazzer_thunk$(raw_msg: $pbr$::RawMessage) -> bool;
                     fn $clearer_thunk$(raw_msg: $pbr$::RawMessage);
                   )rs");
               }
             }}},
           R"rs(
          $with_presence_fields_thunks$
          fn $getter_thunk$(raw_msg: $pbr$::RawMessage) -> $pbr$::PtrAndLen;
          $setter$
        )rs");
}

void SingularString::InThunkCc(Context& ctx,
                               const FieldDescriptor& field) const {
  ctx.Emit({{"field", cpp::FieldName(&field)},
            {"QualifiedMsg", cpp::QualifiedClassName(field.containing_type())},
            {"hazzer_thunk", ThunkName(ctx, field, "has")},
            {"getter_thunk", ThunkName(ctx, field, "get")},
            {"setter_thunk", ThunkName(ctx, field, "set")},
            {"clearer_thunk", ThunkName(ctx, field, "clear")},
            {"with_presence_fields_thunks",
             [&] {
               if (field.has_presence()) {
                 ctx.Emit(R"cc(
                   bool $hazzer_thunk$($QualifiedMsg$* msg) {
                     return msg->has_$field$();
                   }
                   void $clearer_thunk$($QualifiedMsg$* msg) { msg->clear_$field$(); }
                 )cc");
               }
             }}},
           R"cc(
             $with_presence_fields_thunks$;
             ::google::protobuf::rust::PtrAndLen $getter_thunk$($QualifiedMsg$* msg) {
               absl::string_view val = msg->$field$();
               return ::google::protobuf::rust::PtrAndLen(val.data(), val.size());
             }
             void $setter_thunk$($QualifiedMsg$* msg, std::string* s) {
               msg->set_$field$(std::move(*s));
               delete s;
             }
           )cc");
}

}  // namespace rust
}  // namespace compiler
}  // namespace protobuf
}  // namespace google
