// Protocol Buffers - Google's data interchange format
// Copyright 2008 Google Inc.  All rights reserved.
//
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file or at
// https://developers.google.com/open-source/licenses/bsd

// Author: kenton@google.com (Kenton Varda)
//  Based on original Protocol Buffers design by
//  Sanjay Ghemawat, Jeff Dean, and others.

#include "google/protobuf/repeated_ptr_field.h"

#include <algorithm>
#include <cstddef>
#include <cstdint>
#include <cstring>
#include <limits>
#include <string>

#include "absl/base/prefetch.h"
#include "absl/log/absl_check.h"
#include "google/protobuf/arena.h"
#include "google/protobuf/message_lite.h"
#include "google/protobuf/port.h"
#include "google/protobuf/repeated_field.h"

// Must be included last.
#include "google/protobuf/port_def.inc"

namespace google {
namespace protobuf {

namespace internal {

void** RepeatedPtrFieldBase::InternalExtend(int extend_amount) {
  ABSL_DCHECK(extend_amount > 0);
  constexpr size_t ptr_size = sizeof(rep()->elements[0]);
  int capacity = Capacity();
  int new_capacity = capacity + extend_amount;
  Arena* arena = GetArena();
  new_capacity = internal::CalculateReserveSize<void*, kRepHeaderSize>(
      capacity, new_capacity);
  ABSL_CHECK_LE(
      static_cast<int64_t>(new_capacity),
      static_cast<int64_t>(
          (std::numeric_limits<size_t>::max() - kRepHeaderSize) / ptr_size))
      << "Requested size is too large to fit into size_t.";
  size_t bytes = kRepHeaderSize + ptr_size * new_capacity;
  Rep* new_rep;
  if (arena == nullptr) {
    internal::SizedPtr res = internal::AllocateAtLeast(bytes);
    new_capacity = static_cast<int>((res.n - kRepHeaderSize) / ptr_size);
    new_rep = reinterpret_cast<Rep*>(res.p);
  } else {
    new_rep = reinterpret_cast<Rep*>(Arena::CreateArray<char>(arena, bytes));
  }

  if (using_sso()) {
    new_rep->allocated_size = tagged_rep_or_elem_ != nullptr ? 1 : 0;
    new_rep->elements[0] = tagged_rep_or_elem_;
  } else {
    Rep* old_rep = rep();
    memcpy(new_rep, old_rep,
           old_rep->allocated_size * ptr_size + kRepHeaderSize);

    size_t old_size = capacity * ptr_size + kRepHeaderSize;
    if (arena == nullptr) {
      internal::SizedDelete(old_rep, old_size);
    } else {
      arena->ReturnArrayMemory(old_rep, old_size);
    }
  }

  tagged_rep_or_elem_ =
      reinterpret_cast<void*>(reinterpret_cast<uintptr_t>(new_rep) + 1);
  capacity_proxy_ = new_capacity - kSSOCapacity;
  return &new_rep->elements[current_size_];
}

void RepeatedPtrFieldBase::Reserve(int capacity) {
  int delta = capacity - Capacity();
  if (delta > 0) {
    InternalExtend(delta);
  }
}

void RepeatedPtrFieldBase::DestroyProtos() {
  PROTOBUF_ALWAYS_INLINE_CALL Destroy<GenericTypeHandler<MessageLite>>();

  // TODO:  Eliminate this store when invoked from the destructor,
  // since it is dead.
  tagged_rep_or_elem_ = nullptr;
}

void* RepeatedPtrFieldBase::AddMessageLite(ElementFactory factory) {
  return AddInternal(factory);
}

void* RepeatedPtrFieldBase::AddString() {
  return AddInternal([](Arena* arena) { return NewStringElement(arena); });
}

void RepeatedPtrFieldBase::CloseGap(int start, int num) {
  if (using_sso()) {
    if (start == 0 && num == 1) {
      tagged_rep_or_elem_ = nullptr;
    }
  } else {
    // Close up a gap of "num" elements starting at offset "start".
    Rep* r = rep();
    for (int i = start + num; i < r->allocated_size; ++i)
      r->elements[i - num] = r->elements[i];
    r->allocated_size -= num;
  }
  ExchangeCurrentSize(current_size_ - num);
}

MessageLite* RepeatedPtrFieldBase::AddMessage(const MessageLite* prototype) {
  return static_cast<MessageLite*>(
      AddInternal([prototype](Arena* a) { return prototype->New(a); }));
}

void InternalOutOfLineDeleteMessageLite(MessageLite* message) {
  delete message;
}

template PROTOBUF_EXPORT_TEMPLATE_DEFINE void
memswap<ArenaOffsetHelper<RepeatedPtrFieldBase>::value>(
    char* PROTOBUF_RESTRICT, char* PROTOBUF_RESTRICT);

template <>
void RepeatedPtrFieldBase::MergeFrom<std::string>(
    const RepeatedPtrFieldBase& from) {
  ABSL_DCHECK_NE(&from, this);
  int new_size = current_size_ + from.current_size_;
  auto dst = reinterpret_cast<std::string**>(InternalReserve(new_size));
  auto src = reinterpret_cast<std::string* const*>(from.elements());
  auto end = src + from.current_size_;
  auto end_assign = src + std::min(ClearedCount(), from.current_size_);
  for (; src < end_assign; ++dst, ++src) {
    (*dst)->assign(**src);
  }
  if (Arena* const arena = arena_) {
    for (; src < end; ++dst, ++src) {
      *dst = Arena::Create<std::string>(arena, **src);
    }
  } else {
    for (; src < end; ++dst, ++src) {
      *dst = new std::string(**src);
    }
  }
  ExchangeCurrentSize(new_size);
  if (new_size > allocated_size()) {
    rep()->allocated_size = new_size;
  }
}


int RepeatedPtrFieldBase::MergeIntoClearedMessages(
    const RepeatedPtrFieldBase& from) {
  auto dst = reinterpret_cast<MessageLite**>(elements() + current_size_);
  auto src = reinterpret_cast<MessageLite* const*>(from.elements());
  int count = std::min(ClearedCount(), from.current_size_);
  for (int i = 0; i < count; ++i) {
    ABSL_DCHECK(src[i] != nullptr);
    ABSL_DCHECK(TypeId::Get(*src[i]) == TypeId::Get(*src[0]))
        << src[i]->GetTypeName() << " vs " << src[0]->GetTypeName();
    dst[i]->CheckTypeAndMergeFrom(*src[i]);
  }
  return count;
}

void RepeatedPtrFieldBase::MergeFromConcreteMessage(
    const RepeatedPtrFieldBase& from, CopyFn copy_fn) {
  ABSL_DCHECK_NE(&from, this);
  int new_size = current_size_ + from.current_size_;
  void** dst = InternalReserve(new_size);
  const void* const* src = from.elements();
  auto end = src + from.current_size_;
  if (PROTOBUF_PREDICT_FALSE(ClearedCount() > 0)) {
    int recycled = MergeIntoClearedMessages(from);
    dst += recycled;
    src += recycled;
  }
  Arena* arena = GetArena();
  for (; src < end; ++src, ++dst) {
    *dst = copy_fn(arena, *src);
  }
  ExchangeCurrentSize(new_size);
  if (new_size > allocated_size()) {
    rep()->allocated_size = new_size;
  }
}

template <>
void RepeatedPtrFieldBase::MergeFrom<MessageLite>(
    const RepeatedPtrFieldBase& from) {
  ABSL_DCHECK_NE(&from, this);
  ABSL_DCHECK(from.current_size_ > 0);
  int new_size = current_size_ + from.current_size_;
  auto dst = reinterpret_cast<MessageLite**>(InternalReserve(new_size));
  auto src = reinterpret_cast<MessageLite const* const*>(from.elements());
  auto end = src + from.current_size_;
  const MessageLite* prototype = src[0];
  ABSL_DCHECK(prototype != nullptr);
  if (PROTOBUF_PREDICT_FALSE(ClearedCount() > 0)) {
    int recycled = MergeIntoClearedMessages(from);
    dst += recycled;
    src += recycled;
  }
  Arena* arena = GetArena();
  for (; src < end; ++src, ++dst) {
    ABSL_DCHECK(*src != nullptr);
    ABSL_DCHECK(TypeId::Get(**src) == TypeId::Get(*prototype))
        << (**src).GetTypeName() << " vs " << prototype->GetTypeName();
    *dst = prototype->New(arena);
    (*dst)->CheckTypeAndMergeFrom(**src);
  }
  ExchangeCurrentSize(new_size);
  if (new_size > allocated_size()) {
    rep()->allocated_size = new_size;
  }
}

void* NewStringElement(Arena* arena) {
  return Arena::Create<std::string>(arena);
}

}  // namespace internal
}  // namespace protobuf
}  // namespace google

#include "google/protobuf/port_undef.inc"
