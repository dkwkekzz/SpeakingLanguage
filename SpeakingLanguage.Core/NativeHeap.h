#pragma once
#include "iterator.h"
#include "IAllocator.h"

namespace SpeakingLanguage {
	namespace Core {

		class NativeHeap : public IAllocator
		{
		public:
			explicit NativeHeap(int capacity);
			virtual ~NativeHeap();

			bool Empty() const { return _head == 0; }
			bool Resize(int capacity);
			void Reset();

			virtual slChunk* Alloc(int size) override;

			static void Swap(NativeHeap& lhs, NativeHeap& rhs);

		protected:
			uint64_t getIntPtr() const { return (uint64_t)_root; }
			int getHead() const { return _head; }
			int getCapacity() const { return _capacity; }

		private:
			void* _root;
			int _head;
			int _capacity;
		};
	}
}