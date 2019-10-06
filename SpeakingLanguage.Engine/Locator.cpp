#include "stdafx.h"
#include "Locator.h"

using namespace SpeakingLanguage;

Locator::Provider<IInjector<Interaction>>	Locator::InteractionInjector;
Locator::Provider<IInjector<ActionSource>>	Locator::ActionInjector;
Locator::Provider<IProcessor>				Locator::Processor;
Locator::Provider<ICollector<ISystem>>		Locator::SystemCollector;

Locator::Locator()
{
}


Locator::~Locator()
{
}
