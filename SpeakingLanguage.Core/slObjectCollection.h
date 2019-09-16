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

			struct Result
			{
				slObject* subject;
				bool resized;

				Result() : subject(nullptr), resized(false) {}
				Result(slObject* ptr) : subject(ptr), resized(false) {}
				operator slObject*() { return subject; }
			};

			explicit slObjectCollection(int defaultObjectCount);
			~slObjectCollection();

			int GetCount() const;
			Result Find(const slObject::THandle) const;
			Result CreateFront(slObject::THandle);
			Result CreateBack();
			Result InsertFront(const BYTE*, const int, const int);
			Result InsertBack(const BYTE*, const int, const int);
			void Destroy(slObject*);
			void SwapBuffer();

		private:
			struct Impl;
			std::unique_ptr<Impl> _pImpl;

			bool resized;
		};
	}
}

