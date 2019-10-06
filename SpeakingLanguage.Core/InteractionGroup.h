#pragma once
#include "slObject.h"
#include "iterator.h"

namespace SpeakingLanguage {
	namespace Core {

		struct InteractPair
		{
			slObject::THandle handle;
			int count;
		};

		class InteractionGroup
		{
		public:
			using const_iterator = std::vector<InteractPair>::const_iterator;

			explicit InteractionGroup() = default;
			explicit InteractionGroup(const std::vector<InteractPair>* pairs, int begin, int end);

			inline const_iterator begin() const { return _pairs->begin() + _begin; }
			inline const_iterator end() const { return _pairs->begin() + _end; }

			inline int GetCount() const { return _end - _begin; }

		private:
			const std::vector<InteractPair>* _pairs;
			const int _begin{ 0 };
			const int _end{ 0 };
		};

	}
}
