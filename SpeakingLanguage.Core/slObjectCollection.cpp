#include "stdafx.h"
#include "slObjectCollection.h"
#include "ObjectHeap.h"

using namespace SpeakingLanguage::Core;

struct slObjectCollection::Impl
{
	ObjectHeap<slObject> readStack;
	ObjectHeap<slObject> writeStack;
	std::unordered_map<slObject::THandle, slObject*> lookup;

	int maxObjectNum;

	Impl(int defaultObjectCount);
	inline int GenerateHandle() { return maxObjectNum++; }
};

slObjectCollection::Impl::Impl(int defaultObjectCount) :
	readStack(defaultObjectCount * 1024),
	writeStack(defaultObjectCount * 1024),
	lookup(defaultObjectCount)
{
}


slObjectCollection::slObjectCollection(int defaultObjectCount) : _pImpl(std::make_unique<slObjectCollection::Impl>(defaultObjectCount))
{
}

slObjectCollection::~slObjectCollection()
{
}

slObjectCollection::iterator
slObjectCollection::begin()
{
	return _pImpl->readStack.begin();
}

slObjectCollection::iterator
slObjectCollection::end()
{
	return _pImpl->readStack.end();
}

slObjectCollection::const_iterator
slObjectCollection::begin() const
{
	return _pImpl->readStack.cbegin();
}

slObjectCollection::const_iterator
slObjectCollection::end() const
{
	return _pImpl->readStack.cend();
}

int
slObjectCollection::GetCount() const
{
	return _pImpl->lookup.size();
}

slObject* 
slObjectCollection::Find(const slObject::THandle handle) const
{
	return _pImpl->lookup[handle];
}

slObject*
slObjectCollection::CreateFront(slObject::THandle handle)
{
	if (handle == 0)
		handle = _pImpl->GenerateHandle();

	auto* pObj = slObject::ConstructDefault(&_pImpl->readStack, handle);
	if (nullptr != pObj) 
		_pImpl->lookup.emplace(handle, pObj);

	return pObj;
}

slObject*
slObjectCollection::CreateBack()
{
	const auto handle = _pImpl->GenerateHandle();
	auto* pObj = slObject::ConstructDefault(&_pImpl->writeStack, handle);
	if (nullptr != pObj)
		_pImpl->lookup.emplace(handle, pObj);

	return pObj;
}

slObject* 
slObjectCollection::InsertFront(const BYTE* buffer, const int position, const int length)
{
	auto* chk = _pImpl->readStack.Alloc(length);
	if (nullptr == chk) return nullptr;

	auto* pObj = chk->Get<slObject>();
	auto* ret = memcpy(pObj, buffer + position, length);
	if (nullptr == ret) return nullptr;

	const auto handle = pObj->GetHandle();
	_pImpl->lookup.emplace(handle, pObj);

	return pObj;
}

slObject* 
slObjectCollection::InsertBack(const BYTE* buffer, const int position, const int length)
{
	auto* chk = _pImpl->writeStack.Alloc(length);
	if (nullptr == chk) return nullptr;

	auto* pObj = chk->Get<slObject>();
	auto* ret = memcpy(pObj, buffer + position, length);
	if (nullptr == ret) return nullptr;

	return pObj;
}

void 
slObjectCollection::Destroy(slObject* obj)
{
	_pImpl->lookup.erase(obj->GetHandle());
	obj->Release();
}

void
slObjectCollection::SwapBuffer()
{
	for (auto& pObj : _pImpl->writeStack)
	{
		const auto handle = pObj.GetHandle();
		if (0 == handle) continue;

		_pImpl->lookup.emplace(handle, &pObj);
	}

	ObjectHeap<slObject>::Swap(_pImpl->readStack, _pImpl->writeStack);
	_pImpl->writeStack.Reset();
}
