#pragma once
#include "SimpleSystem.h"

namespace SpeakingLanguage
{
	struct FactoryHelper
	{
		template<typename TSystem>
		static std::unique_ptr<ISystem> CreateSystem(const ActionSource& action)
		{
			auto ptr = std::make_unique<TSystem>(action);
			return ptr;
		}
	};
}