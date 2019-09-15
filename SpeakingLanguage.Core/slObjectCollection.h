#pragma once
#include "iterator.h"
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
			slObject* Find(const slObject::THandle) const;
			slObject* CreateFront(slObject::THandle);
			slObject* CreateBack();
			slObject* InsertFront(const BYTE*, const int, const int);
			slObject* InsertBack(const BYTE*, const int, const int);
			void Destroy(slObject*);
			void SwapBuffer();

		private:
			struct Impl;
			std::unique_ptr<Impl> _pImpl;
		};
	}
}

