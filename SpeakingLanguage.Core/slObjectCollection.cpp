#include "stdafx.h"
#include "slObjectCollection.h"
#include "ObjectHeap.h"

using namespace SpeakingLanguage::Core;

class LinearLookup
{
public:
	LinearLookup(int defaultObjectCount);

	inline int GetCount() { return lookup.size() - _freeCount; }
	inline int GetThreshold() { return lookup.size(); }
	inline slObject::THandle PublishHandle() { return _freeCount > 0 ? _freeList : lookup.size(); }
	inline slObject* operator[](slObject::THandle handle) { return lookup[handle].second; }

	bool Insert(slObject* pObj, bool& resized);
	bool Delete(slObject* pObj);

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

bool 
LinearLookup::Insert(slObject* pObj, bool& resized)
{
	const auto handle = pObj->GetHandle();
	if (0 == handle) return false;

	if (_freeList == handle)
	{
		auto& freeNode = lookup[_freeList];
		_freeList = freeNode.first;
		_freeCount--;
	}

	const int max = lookup.size();
	if (max <= handle)
	{
		int newMax = max;
		while (newMax <= handle) newMax <<= 1;

		lookup.resize(newMax);
		resized = true;

		for (int i = max; i != newMax; i++)
		{
			auto& freeNode = lookup[i];
			freeNode.first = _freeList;
			freeNode.second = nullptr;
			_freeList = i;
		}
	}

	lookup[handle] = std::move(std::make_pair(handle, pObj));
	return true;
}

bool 
LinearLookup::Delete(slObject* pObj)
{
	const auto handle = pObj->GetHandle();
	if (0 == handle) return false;

	const int max = lookup.size();
	if (max <= handle) return false;

	auto& freeNode = lookup[handle];
	freeNode.first = _freeList;
	freeNode.second = nullptr;
	_freeList = handle;
	_freeCount++;

	return true;
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

slObjectCollection::Result
slObjectCollection::Find(const slObject::THandle handle) const
{
	return _pImpl->lookup[handle];
}

slObjectCollection::Result
slObjectCollection::CreateFront(slObject::THandle handle)
{
	slObjectCollection::Result res;

	if (handle == 0)
		handle = _pImpl->lookup.PublishHandle();

	res.subject = slObject::ConstructDefault(&_pImpl->readStack, handle);
	if (nullptr != res.subject)
		_pImpl->lookup.Insert(res.subject, res.resized);

	return res;
}

slObjectCollection::Result
slObjectCollection::CreateBack()
{
	slObjectCollection::Result res;

	const auto handle = _pImpl->lookup.PublishHandle();
	res.subject = slObject::ConstructDefault(&_pImpl->writeStack, handle);
	if (nullptr != res.subject)
		_pImpl->lookup.Insert(res.subject, res.resized);

	return res;
}

slObjectCollection::Result
slObjectCollection::InsertFront(const BYTE* buffer, const int position, const int length)
{
	auto* chk = _pImpl->readStack.Alloc(length);
	if (nullptr == chk) return nullptr;

	slObjectCollection::Result res;

	res.subject = chk->Get<slObject>();
	auto* ret = memcpy(res.subject, buffer + position, length);
	if (nullptr == ret) return nullptr;

	_pImpl->lookup.Insert(res.subject, res.resized);
	return res;
}

slObjectCollection::Result
slObjectCollection::InsertBack(const BYTE* buffer, const int position, const int length)
{
	auto* chk = _pImpl->writeStack.Alloc(length);
	if (nullptr == chk) return nullptr;

	slObjectCollection::Result res;

	res.subject = chk->Get<slObject>();
	auto* ret = memcpy(res.subject, buffer + position, length);
	if (nullptr == ret) return nullptr;

	return res;
}

void 
slObjectCollection::Destroy(slObject* obj)
{
	_pImpl->lookup.Delete(obj);
	obj->Release();
}

void
slObjectCollection::SwapBuffer()
{
	bool resized = false;
	for (auto& obj : _pImpl->writeStack)
	{
		const auto handle = obj.GetHandle();
		if (0 == handle) continue;

		_pImpl->lookup.Insert(&obj, resized);
	}

	ObjectHeap<slObject>::Swap(_pImpl->readStack, _pImpl->writeStack);
	_pImpl->writeStack.Reset();
}
