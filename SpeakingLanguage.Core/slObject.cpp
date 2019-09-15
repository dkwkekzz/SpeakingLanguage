#include "stdafx.h"
#include "slObject.h"
#include "IAllocator.h"
#include "Basic.h"

using namespace SpeakingLanguage::Core;

slObject::slObject()
{
}

slObject::~slObject() 
{
}

slObject*
slObject::ConstructDefault(IAllocator* allocator, slObject::THandle handle)
{
	const int szObj = sizeof(slObject);
	auto* chk = allocator->Alloc(szObj);
	if (nullptr == chk) return false;

	auto* objPtr = chk->Get<slObject>();
	objPtr->_handle = handle;

	const int szChk = sizeof(slChunk);
	const int szBasic = sizeof(State::Basic);
	auto* basicChk = allocator->Alloc(szBasic);
	if (nullptr == basicChk) return false;

	objPtr->_capacity += szChk + szBasic;
	return objPtr;
}
