#include "stdafx.h"
#include "InteractionGroup.h"

using namespace SpeakingLanguage::Core;

InteractionGroup::InteractionGroup(const std::vector<InteractPair>* const pairs, int begin, int end) : _pairs(pairs), _begin(begin), _end(end)
{
}
