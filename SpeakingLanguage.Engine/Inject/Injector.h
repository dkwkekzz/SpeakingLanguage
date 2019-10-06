#pragma once
#include "Interaction.h"
#include "ActionSource.h"
#include "Locator.h"

namespace SpeakingLanguage
{
	class Injector : public IInjector<ActionSource>, public IInjector<Interaction>
	{
	public:
		explicit Injector();
		~Injector();

		Result<void> Insert(const ActionSource& action);
		Result<void> Insert(const Interaction& interaction);

	private:
		struct objectPool;
		struct systemNode;
		std::unique_ptr<objectPool> _objs;
		std::unique_ptr<systemNode> _root;
	};
}