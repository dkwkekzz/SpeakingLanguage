#pragma once
#include "iterator.h"
#include "slObject.h"

namespace SpeakingLanguage {
	namespace Core {

		struct Interaction
		{
			slObject::THandle subject;
			slObject::THandle target;
		};

		struct InteractionGroup
		{

		};

		class InteractionGraph
		{
		public:
			explicit InteractionGraph(int defaultObjectCount);
			~InteractionGraph();

			void Insert(const Interaction);
			void Reset();
			bool TryGetInteractGroup(const_iterator<slObject>&, int, InteractionGroup&);

		private:
			struct Impl;
			std::unique_ptr<Impl> _pImpl;
		};
	}
}
