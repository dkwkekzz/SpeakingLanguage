#include "stdafx.h"
#include "Locator.h"

using namespace SpeakingLanguage;

Locator::Provider<IInjector<Interaction>>	Locator::InteractionInjector;
Locator::Provider<IInjector<ActionSource>>	Locator::ActionInjector;

Locator::Locator()
{
}


Locator::~Locator()
{
}
