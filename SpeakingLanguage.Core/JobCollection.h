#pragma once
#include "Result.h"
#include "Interactor.h"

namespace SpeakingLanguage {
	namespace Core{

		class JobCollection
		{
		public:
			inline Result<void> Interact(Service& service, InteractionGroup& group) { return _interactor.Execute(service, group); }

		private:
			Interactor _interactor;
		};

	}
}
