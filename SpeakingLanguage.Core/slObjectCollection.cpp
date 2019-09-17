#include "stdafx.h"
#include "slObjectCollection.h"
#include "ObjectHeap.h"

using namespace SpeakingLanguage::Core;

class LinearLookup
{
public:
	LinearLookup(int defaultObjectCount);

	inline int GetCount() { return lookup.size() - _freeCount; }
	inline int GetMax() { return lookup.size(); }
	inline slObject::THandle PublishHandle() { return _freeCount > 0 ? _freeList : lookup.size(); }
	inline slObject* operator[](slObject::THandle handle) { return lookup[handle].second; }

	Result<bool> Insert(slObject* pObj);
	Result<void> Delete(slObject* pObj);

private:
	std::vector<std::pair<slObject::THandle, slObject*> > lookup;
	slObject::THandle _freeList{ -1 };
	int _freeCount{ 0 };
};

LinearLookup::LinearLookup(int defaultObjectCount) : lookup(defaultObjectCount)
{
	for (int i = 0; i != defaultObjectCount; i++)
	{
		auto& freeNode = lookup[i];
		freeNode.first = _freeList;
		freeNode.second = nullptr;
		_freeList = i;
	}
}

Result<bool>
LinearLookup::Insert(slObject* pObj)
{
	const auto handle = pObj->GetHandle();
	if (0 == handle) return Error::NullReferenceHandle;

	if (_freeList == handle)
	{
		auto& freeNode = lookup[_freeList];
		_freeList = freeNode.first;
		_freeCount--;

		freeNode.first = handle;
		freeNode.second = pObj;
		return true;
	}

	bool resized = false;

	const int max = lookup.size();
	if (max <= handle)
	{
		int newMax = max;
		while (newMax <= handle) newMax <<= 1;

		lookup.resize(newMax);
		resized = true;

		for (int i = max; i != newMax; i++)
		{
			if (i == handle) continue;

			auto& freeNode = lookup[i];
			freeNode.first = _freeList;
			freeNode.second = nullptr;
			_freeList = i;
		}
	}

	auto& node = lookup[handle];
	node.first = handle;
	node.second = pObj;

	return Done(resized);
}

Result<void>
LinearLookup::Delete(slObject* pObj)
{
	const auto handle = pObj->GetHandle();
	if (0 == handle) return Error::NullReferenceHandle;

	const int max = lookup.size();
	if (max <= handle) return Error::OverflowHandle;

	auto& freeNode = lookup[handle];
	freeNode.first = _freeList;
	freeNode.second = nullptr;
	_freeList = handle;
	_freeCount++;

	return Done();
}

struct slObjectCollection::Impl
{
	ObjectHeap<slObject> readStack;
	ObjectHeap<slObject> writeStack;
	LinearLookup lookup;

	Impl(int defaultObjectCount);
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
	return _pImpl->lookup.GetCount();
}

Result<slObject*>
slObjectCollection::Find(const slObject::THandle handle) const
{
	if (_pImpl->lookup.GetMax() <= handle) return Error::OverflowHandle;

	return _pImpl->lookup[handle];
}

Result<bool>
slObjectCollection::CreateFront(slObject::THandle handle)
{
	if (handle == 0)
		handle = _pImpl->lookup.PublishHandle();

	auto* pObj = slObject::ConstructDefault(&_pImpl->readStack, handle);
	if (nullptr == pObj)
		return Error::NullReferenceHandle;

	return _pImpl->lookup.Insert(pObj);
}

Result<bool>
slObjectCollection::CreateBack()
{
	const auto handle = _pImpl->lookup.PublishHandle();
	auto* pObj = slObject::ConstructDefault(&_pImpl->readStack, handle);
	if (nullptr == pObj)
		return Error::NullReferenceHandle;

	return _pImpl->lookup.Insert(pObj);
}

Result<bool>
slObjectCollection::InsertFront(const BYTE* buffer, const int position, const int length)
{
	auto* chk = _pImpl->readStack.Alloc(length);
	if (nullptr == chk) return Error::OutOfMemory;

	auto* pObj = chk->Get<slObject>();
	auto* ret = memcpy(pObj, buffer + position, length);
	if (nullptr == ret) return Error::OutOfMemory;

	return _pImpl->lookup.Insert(pObj);
}

Result<bool>
slObjectCollection::InsertBack(const BYTE* buffer, const int position, const int length)
{
	auto* chk = _pImpl->writeStack.Alloc(length);
	if (nullptr == chk) return Error::OutOfMemory;

	auto* pObj = chk->Get<slObject>();
	auto* ret = memcpy(pObj, buffer + position, length);
	if (nullptr == ret) return Error::OutOfMemory;

	return true;
}

Result<void>
slObjectCollection::Destroy(slObject* obj)
{
	obj->Release();
	return _pImpl->lookup.Delete(obj);
}

Result<void>
slObjectCollection::SwapBuffer()
{
	for (auto& obj : _pImpl->writeStack)
	{
		const auto handle = obj.GetHandle();
		if (0 == handle) continue;

		auto ret = _pImpl->lookup.Insert(&obj);
		if (!ret.Success()) return ret.error;
	}

	ObjectHeap<slObject>::Swap(_pImpl->readStack, _pImpl->writeStack);
	_pImpl->writeStack.Reset();
}
