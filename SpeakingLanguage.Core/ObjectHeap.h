#pragma once
#include "iterator.h"
#include "slObject.h"
#include "NativeHeap.h"

namespace SpeakingLanguage {
	namespace Core {

		template<typename T>
		class ObjectHeap : public NativeHeap
		{
		public:
			using value_type = T;
			using size_type = size_t;
			using difference_type = ptrdiff_t;
			using pointer = T* ;
			using const_pointer = const T*;
			using reference = T & ;
			using const_reference = const T&;

			using iterator = iterator<slObject>;
			using const_iterator = const_iterator<slObject>;

			inline iterator begin() { return iterator((T*)(getIntPtr() + sizeof(slChunk))); }
			inline iterator end() { return iterator((T*)(getIntPtr() + getHead() + sizeof(slChunk))); }
			inline const_iterator cbegin() const { return const_iterator((T*)(getIntPtr() + sizeof(slChunk))); }
			inline const_iterator cend() const { return const_iterator((T*)(getIntPtr() + getHead() + sizeof(slChunk))); }

			explicit ObjectHeap(int capacity) : NativeHeap(capacity) {}
			~ObjectHeap() {}

			pointer
			address(reference r) const
			{
				return &r;
			}

			const_pointer
			address(const_reference r) const
			{
				return &r;
			}

			pointer
			allocate(size_type n, const void* /*hint*/ = 0)
			{
				const int sz = n * sizeof(T);
				auto* chk = this->Alloc(sz);
				if (chk == nullptr) return nullptr;

				return chk->Get<T>;
			}

			void
			deallocate(pointer p, size_type /*n*/)
			{
				auto* chk = slChunk::GetChunk(p);
				chk->typeHandle = 0;
			}

			void
			construct(pointer p, const T& val)
			{
				new (p) T(val);
			}

			void
			destroy(pointer p)
			{
				p->~T();
			}

			size_type
			max_size() const
			{
				return getCapacity() / sizeof(T);
			}
		};
	}
}