#include "stdafx.h"
#include "InteractionGraph.h"
#include "ObjectHeap.h"
#include "Utils/disjointset.h"

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
			auto* chk = allocator->Alloc(sizeof(HandleBlock<N>));
			if (nullptr == chk) return nullptr;

			next = chk->Get<HandleBlock<N>>();
			next->Initialize();
		}

		return next->FindHead(count - HandleBlock<N>::MAX, allocator);
	}

	void Flush(std::queue<slObject::THandle>& queue, int count)
	{
		for (int i = 0; i < count || i < MAX; i++)
			queue.emplace(vals[i], 0);

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
};

class BlockHead
{
public:
	void Insert(slObject::THandle handle);
	inline void Initialize(IAllocator* allocator) { _allocator = allocator; }
	inline int GetCount() const { return _count; }
	inline void Mark() { _count = 0; }
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
		pairs.emplace_back(new InteractPair{ here, length });
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

bool 
InteractionGraph::TryGetInteractGroup(const_iterator<slObject>& begin, const_iterator<slObject>& end, int capacity, InteractionGroup& group)
{
	int last = _pImpl->pairs.size();
	int count = 0;
	for (auto iter = begin; iter != end; iter++)
	{
		auto& pObj = *iter;
		count += _pImpl->_bfsSelect(pObj.GetHandle());
		if (capacity >= 0 && count >= capacity)
			break;
	}
	
	if (count == 0)
		return false;

	group = new InteractGroup(_pImpl->pairs, last, last + count);
	return true;
}

internal struct InteractionGraph : IDisposable
{
	// splaybt로 변경하자. 용량문제를 해결할 수 있다.
	private sealed class EdgeList : List<slObjectHandle>
	{
		public int Index{ get; }
		public int Order{ get; set; }

			public EdgeList(int index, int capacity) : base(capacity)
		{
			Index = index;
			Order = 0;
		}

		public void Reset()
		{
			base.Clear();
			Order = 0;
		}
	}

	private readonly Dictionary<slObjectHandle, EdgeList> _dicGraph;
	private readonly Queue<EdgeList> _listPool;
	private readonly Queue<slObjectHandle> _queue;
	private readonly int _defaultInteractCount;

	private Library.umnMarshal _allocator;
	private Library.umnArray<InteractPair> _arrPair;

	public InteractionGraph(int defaultObjectCount, int defaultInteractCount)
	{
		_dicGraph = new Dictionary<slObjectHandle, EdgeList>(defaultObjectCount);
		_listPool = new Queue<EdgeList>();
		_queue = new Queue<slObjectHandle>();

		_allocator = new Library.umnMarshal();
		_arrPair = Library.umnArray<InteractPair>.CreateNew(ref _allocator, defaultObjectCount * defaultInteractCount);
		_defaultInteractCount = defaultInteractCount;
	}

	public void Dispose()
	{
		_allocator.Dispose();
	}

	public void Insert(ref Interaction stInter)
	{
		if (!_dicGraph.TryGetValue(stInter.subject, out EdgeList list))
		{
			if (_listPool.Count > 0)
				list = _listPool.Dequeue();
			else
				list = new EdgeList(_dicGraph.Count, _defaultInteractCount);
			_dicGraph.Add(stInter.subject, list);
		}

		// [!] 성능상 문제. 
		if (!list.Contains(stInter.target))
			list.Add(stInter.target);
	}

	public bool Remove(slObjectHandle subject)
	{
		_listPool.Enqueue(_dicGraph[subject]);
		return _dicGraph.Remove(subject);
	}

	public void Reset()
	{
		var iter = _dicGraph.Values.GetEnumerator();
		while (iter.MoveNext())
		{
			iter.Current.Reset();
		}
		_arrPair.Clear();
	}

	public unsafe bool TryGetInteractGroup(ref slObjectCollection.Enumerator objIter, int capacity, out InteractGroup group
		{
			int begin = _arrPair.Length;
			int count = 0;
			while (objIter.MoveNext())
			{
				var pObj = objIter.Current;
				count += _bfsSelect(pObj->handle);
				if (capacity >= 0 && count >= capacity)
					break;
			}

			if (count == 0)
			{
				group = default;
				return false;
			}

			group = new InteractGroup(_arrPair.GetIndexer(), begin, begin + count);
			return true;
		}

		private unsafe int _bfsSelect(slObjectHandle first)
		{
			Library.Tracer.Assert(_queue.Count == 0);

			int count = 0;

			_queue.Enqueue(first);
			while (_queue.Count > 0)
			{
				var here = _queue.Dequeue();
				int length = 0;
				_arrPair.PushBack(new InteractPair(here, length));
				count++;

				if (!_dicGraph.TryGetValue(here, out EdgeList thereList))
					continue;

				length = thereList.Count;
				if (thereList.Order > 0)
					continue;
				thereList.Order = 1;

				for (int i = 0; i != length; i++)
				{
					var there = thereList[i];
					_queue.Enqueue(there);
				}
			}

			return count;
		}
}