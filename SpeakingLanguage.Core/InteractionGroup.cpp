#include "stdafx.h"
#include "InteractionGroup.h"

using namespace SpeakingLanguage::Core;

InteractPair::InteractPair() : handle(-1), count(0)
{
}

InteractPair::InteractPair(const slObject::THandle h, int c) : handle(h), count(c)
{
}

InteractionGroup::InteractionGroup(const std::vector<InteractPair>* pairs, int begin, int end) : _pairs(pairs), _begin(begin), _end(end)
{
}
