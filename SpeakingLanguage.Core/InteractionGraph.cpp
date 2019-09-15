#include "stdafx.h"
#include "InteractionGraph.h"
#include "ObjectHeap.h"
#include "Utils/disjointset.h"
#include "Utils/splay.h"

using namespace SpeakingLanguage::Core;

struct InteractPair
{
	slObject::THandle handle;
	int count;
};

template <>
struct HandleBlock<4>
{
public:
	constexpr int Max() const { return 4; }
	inline int GetCount() const { return count; }
	void Insert(slObject::THandle handle, IAllocator* allocator);
	slObject::THandle* current{ &vals[0] };

private:
	int count{ 0 };
	slObject::THandle vals[4 + 1]{ 0, 0, 0, 0, -1 };
	slObject::THandle* current{ &vals[0] };
	HandleBlock<8>* next{ nullptr };
};

template<int N>
class HandleBlock
{
public:
	HandleBlock() { vals[N] = -1; }

	constexpr int Max() const { return N; }
	inline int GetCount() const { return count; }
	void Insert(slObject::THandle handle, IAllocator* allocator);

private:
	int count{ 0 };
	slObject::THandle vals[N + 1];
	HandleBlock<N * 2>* next;
};

template<int N>
void 
HandleBlock<N>::Insert(slObject::THandle handle, IAllocator* allocator)
{
	if (count == N)
	{
		if (!next)
		{

			next =

		}
			next->Insert(handle);
	}

	vals[count++] = handle;
	return true;
}

template <>
struct HandleBlock<256> 
{

};

struct InteractionGraph::Impl
{
	using HandleBlock = std::deque<slObject::THandle, ObjectHeap<slObject::THandle> >;

	ObjectHeap<slObject::THandle> heap;
	Utils::disjointset groupSet;
	std::vector<HandleBlock> linkMap;
	std::queue<slObject::THandle, HandleBlock> queue;
	std::vector<InteractPair, ObjectHeap<slObject::THandle>> pairs;

	Impl(int defaultObjectCount);

	int _bfsSelect(slObject::THandle first);
};

InteractionGraph::Impl::Impl(int defaultObjectCount) :
	heap(defaultObjectCount * 1024),
	groupSet(defaultObjectCount),
	linkMap(defaultObjectCount),
	pairs(defaultObjectCount, heap)
{
}

int
InteractionGraph::Impl::_bfsSelect(slObject::THandle first)
{
	int count = 0;

	queue.push(first);
	while (!queue.empty())
	{
		const auto here = queue.front();
		int length = 0;
		pairs.emplace_back(new InteractPair{ here, length });
		count++;

		if (linkMap.count(here) == 0) continue;

		length = thereList.Count;
		if (thereList.Order > 0)
			continue;
		thereList.Order = 1;

		for (int i = 0; i != length; i++)
		{
			var there = thereList[i];
			queue.push(there);
		}
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
InteractionGraph::Insert(const Interaction stInter)
{
	_pImpl->groupSet.Merge(stInter.subject, stInter.target);
	
	Utils::splay<slObject::THandle>* splay;
	auto& map = _pImpl->linkMap;
	if (map.count(stInter.subject) == 0)
	{
		auto* chk = _pImpl->heap.Alloc(sizeof(Utils::splay<slObject::THandle>));
		splay = chk->Get<Utils::splay<slObject::THandle>>();
		map.emplace(stInter.subject, splay);
	}
	else
	{
		splay = map[stInter.subject];
	}

	splay->Insert(stInter.target);
}

void
InteractionGraph::Reset()
{
}

bool 
InteractionGraph::TryGetInteractGroup(const_iterator<slObject>& begin, const_iterator<slObject>& end, int capacity, InteractionGroup& group)
{
	int last = _arrPair.Length;
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

	group = new InteractGroup(_arrPair.GetIndexer(), last, last + count);
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