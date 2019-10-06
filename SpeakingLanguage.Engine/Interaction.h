#pragma once
#include "slObject.h"

namespace SpeakingLanguage
{
	struct Interaction
	{
		slObject::THandle subject;
		slObject::THandle target;
	};
}