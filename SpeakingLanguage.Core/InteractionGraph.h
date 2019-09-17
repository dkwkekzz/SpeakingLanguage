#pragma once
#include "iterator.h"
#include "slObject.h"
#include "Result.h"

namespace SpeakingLanguage {
	namespace Core {

		struct Interaction
		{
			slObject::THandle subject;
			slObject::THandle target;
		};

		struct InteractPair
		{
			slObject::THandle handle;
			int count;
		};

		struct InteractionGroup
		{
			std::vector<InteractPair>* pPairs;

			InteractionGroup()
			{

			}
		};

		class InteractionGraph
		{
		public:
			explicit InteractionGraph(int defaultObjectCount);
			~InteractionGraph();

			Result<void> Insert(const Interaction);
			void Resize(int);
			void Reset();
			bool TryGetInteractGroup(const_iterator<slObject>&, const_iterator<slObject>&, int, InteractionGroup&);

		private:
			struct Impl;
			std::unique_ptr<Impl> _pImpl;
		};
	}
}
