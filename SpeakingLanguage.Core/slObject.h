#pragma once
#include "iterator.h"
#include "slChunk.h"

namespace SpeakingLanguage {
	namespace Core {

		class IAllocator;

		class slObject
		{
		public:
			using THandle = int;
			using iterator = iterator<slChunk>;
			using const_iterator = const_iterator<slChunk>;

			explicit slObject();
			~slObject();

			inline iterator begin() { return iterator((slChunk*)((uint64_t)this + sizeof(slObject))); }
			inline iterator end() { return iterator((slChunk*)((uint64_t)this + sizeof(slObject) + _capacity)); }
			inline const_iterator cbegin() const { return const_iterator((slChunk*)((uint64_t)this + sizeof(slObject))); }
			inline const_iterator cend() const { return const_iterator((slChunk*)((uint64_t)this + sizeof(slObject) + _capacity)); }
			inline THandle GetHandle() const { return _handle; }
			inline slObject* GetNext() const { return (slObject*)((uint64_t)this + sizeof(slObject) + _capacity + sizeof(slChunk)); }
			inline void Release() { _handle = 0; }

			static slObject* ConstructDefault(IAllocator* allocator, slObject::THandle handle);

		private:
			THandle _handle{ 0 };
			int _capacity{ 0 };
		};

	}
}