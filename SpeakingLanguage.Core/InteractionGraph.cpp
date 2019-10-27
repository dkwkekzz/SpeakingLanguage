#include "stdafx.h"
#include "InteractionGraph.h"
#include "ObjectHeap.h"
#include "Utils/disjointset.h"

using namespace SpeakingLanguage;
using namespace SpeakingLanguage::Core;

template<int N>
struct HandleBlock
{
	enum { MAX = N };

	slObject::THandle vals[N + 1];
	HandleBlock<N << 1>* next;

	inline void Initialize() { next = nullptr; vals[N] = -1; }

	slObject::THandle* FindHead(int count, IAllocator* allocator)
	{
		if (count < HandleBlock<N>::MAX)
			return &vals[0] + count;

		if (next == nullptr)
		{
			auto* chk = allocator->Alloc(sizeof(HandleBlock<N << 1>));
			if (nullptr == chk) return nullptr;

			next = chk->Get<HandleBlock<N << 1>>();
			next->Initialize();
		}

		return next->FindHead(count - HandleBlock<N>::MAX, allocator);
	}

	void Flush(std::queue<slObject::THandle>& queue, int count)
	{
		for (int i = 0; i < count && i < MAX; i++)
			queue.emplace(vals[i]);

		if (count > MAX)
			next->Flush(queue, count - MAX);
	}
};

template <>
struct HandleBlock<256>
{
	enum { MAX = 256 };

	slObject::THandle vals[256 + 1];
	HandleBlock<256>* next;

	inline void Initialize() { next = nullptr; vals[256] = -1; }

	slObject::THandle* FindHead(int count, IAllocator* allocator)
	{
		if (count < HandleBlock<256>::MAX)
			return &vals[0] + count;

		if (next == nullptr)
		{
			auto* chk = allocator->Alloc(sizeof(HandleBlock<256>));
			if (nullptr == chk) return nullptr;

			next = chk->Get<HandleBlock<256>>();
			next->Initialize();
		}

		return next->FindHead(count - HandleBlock<256>::MAX, allocator);
	}

	void Flush(std::queue<slObject::THandle>& queue, int count)
	{
		for (int i = 0; i < count && i < MAX; i++)
			queue.emplace(vals[i]);

		if (count > MAX)
			next->Flush(queue, count - MAX);
	}
};

class BlockHead
{
public:
	void Insert(slObject::THandle handle);
	inline void Initialize(IAllocator* allocator) { _allocator = allocator; }
	inline int GetCount() const { return _count; }
	inline void Flush(std::queue<slObject::THandle>& queue) 
	{ 
		_next->Flush(queue, _count); 
		_count = 0; 
		_next = nullptr;
	}

private:
	int _count{ 0 };
	slObject::THandle* _begin;
	slObject::THandle* _head;
	HandleBlock<4>* _next;
	IAllocator* _allocator;
};

void
BlockHead::Insert(slObject::THandle handle)
{
	if (nullptr == _next)
	{
		auto* chk = _allocator->Alloc(sizeof(HandleBlock<4>));
		if (nullptr == chk) return;

		_next = chk->Get<HandleBlock<4>>();
		_begin = &_next->vals[0];
		_head = &_next->vals[0];
	}
	else
	{
		if (*(++_head) == -1)
			_head = _next->FindHead(_count, _allocator);
	}

	*_head = handle;
}

struct InteractionGraph::Impl
{
	//Utils::disjointset groupSet;
	NativeHeap heap;
	std::vector<BlockHead*> linkMap;
	std::queue<slObject::THandle> queue;
	std::vector<InteractPair> pairs;

	Impl(int defaultObjectCount);

	Result<int> _bfsSelect(slObject::THandle first);
};

InteractionGraph::Impl::Impl(int defaultObjectCount) :
	//groupSet(defaultObjectCount),
	heap(defaultObjectCount * 1024),
	linkMap(defaultObjectCount),
	pairs(defaultObjectCount)
{
}

// pair을 count랑 짝지어 넣는 방법보다 더 메모리친화적인방법을 생각해보자.
Result<int>
InteractionGraph::Impl::_bfsSelect(slObject::THandle first)
{
	int count = 0;

	queue.push(first);
	while (!queue.empty())
	{
		const auto here = queue.front();
		auto* header = linkMap[here];
		if (nullptr == header) continue;

		int length = linkMap[here]->GetCount();
		pairs.emplace_back(here, length);
		count++;

		if (length == 0)  continue;
		header->Flush(queue);
	}

	return count;
}

InteractionGraph::InteractionGraph(int defaultObjectCount) : _pImpl(std::make_unique<Impl>(defaultObjectCount))
{
}

InteractionGraph::~InteractionGraph()
{
}

void 
InteractionGraph::Resize(int capacity)
{
	_pImpl->linkMap.resize(capacity);
}

Result<void>
InteractionGraph::Insert(const Interaction stInter)
{
	auto* header = _pImpl->linkMap[stInter.subject];
	if (nullptr == header)
	{
		auto* chk = _pImpl->heap.Alloc(sizeof(BlockHead));
		if (nullptr == chk) return Error::OutOfMemory;

		header = chk->Get<BlockHead>();
		header->Initialize(&_pImpl->heap);
	}

	header->Insert(stInter.target);
	return Done();
}

void
InteractionGraph::Reset()
{
	_pImpl->pairs.clear();
}

Result<InteractionGroup>
InteractionGraph::SelectGroup(iterator<slObject>& current, const iterator<slObject>& end, int capacity)
{
	int last = _pImpl->pairs.size();
	int count = 0;
	for (current; current != end; current++)
	{
		auto& pObj = *current;
		count += _pImpl->_bfsSelect(pObj.GetHandle());
		if (capacity >= 0 && count >= capacity)
			break;
	}
	
	return InteractionGroup(&_pImpl->pairs, last, last + count);
}
