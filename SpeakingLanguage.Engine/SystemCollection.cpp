#include "stdafx.h"
#include "SystemCollection.h"

using namespace SpeakingLanguage;

SystemCollection::SystemCollection()
{
}


SystemCollection::~SystemCollection()
{
}

Result<void> SystemCollection::Add(std::shared_ptr<ISystem> system)
{
	_systems.emplace_back(system);
	return Done();
}