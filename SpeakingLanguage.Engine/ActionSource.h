#pragma once
#include "ISystem.h"

namespace SpeakingLanguage
{
	struct ActionSource
	{
		SyncPair sync;
		ISystem::TAction action;
	};
}