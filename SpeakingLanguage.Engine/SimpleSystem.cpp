#include "stdafx.h"
#include "SimpleSystem.h"

using namespace SpeakingLanguage;

SimpleSystem::SimpleSystem(const ActionSource& action) : ISystem(action.sync), _action(action.action)
{
}

SimpleSystem::~SimpleSystem()
{
}

Result<void> SimpleSystem::Insert(const InteractPair& itr)
{
	return Done();
}
