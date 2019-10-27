#pragma once
#include "iterator.h"
#include "Result.h"
#include "slObject.h"

namespace SpeakingLanguage {
	namespace Core {

		class slObjectCollection
		{
		public:
			using iterator = iterator<slObject>;
			using const_iterator = const_iterator<slObject>;

			iterator begin();
			iterator end();
			const_iterator begin() const;
			const_iterator end() const;

			explicit slObjectCollection(int defaultObjectCount);
			~slObjectCollection();

			int GetCount() const;
			Result<slObject*> Find(const slObject::THandle) const;
			Result<bool> CreateFront(slObject::THandle);
			Result<bool> CreateBack();
			Result<bool> InsertFront(const BYTE*, const int, const int);
			Result<bool> InsertBack(const BYTE*, const int, const int);
			Result<void> Destroy(slObject*);
			Result<void> SwapBuffer();

		private:
			struct Impl;
			std::unique_ptr<Impl> _pImpl;

			bool resized;
		};
	}
}

