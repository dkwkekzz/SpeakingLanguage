#pragma once
#include "iterator.h"
#include "Result.h"
#include "slObject.h"
#include "InteractionGroup.h"

namespace SpeakingLanguage {
	namespace Core {

		struct Interaction
		{
			slObject::THandle subject;
			slObject::THandle target;
		};

		class InteractionGraph
		{
		public:
			explicit InteractionGraph(int defaultObjectCount);
			~InteractionGraph();

			Result<void> Insert(const Interaction);
			void Resize(int);
			void Reset();
			Result<InteractionGroup> SelectGroup(iterator<slObject>&, const iterator<slObject>&, int);

		private:
			struct Impl;
			std::unique_ptr<Impl> _pImpl;
		};
	}
}
