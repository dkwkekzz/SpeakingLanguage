#pragma once
#include "Interaction.h"
#include "Locator.h"

namespace SpeakingLanguage
{
	class Injector
	{
	public:
		explicit Injector();
		~Injector();

		Result<void> Insert(const Interaction& itr);

	private:
		std::map<slObject::THandle, std::vector<slState::THandle>> _stmap;

	};
}