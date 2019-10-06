#include "stdafx.h"
#include "JobPartitioner.h"
#include "slObjectCollection.h"
#include "InteractionGraph.h"

using namespace SpeakingLanguage::Core;

JobPartitioner::JobPartitioner(int jobchunkLength) : _minJobchunkLength(jobchunkLength)
{
}

Result<int>
JobPartitioner::CollectJob(Service& service, int workerCount)
{
	auto* objs = service.GetObjectCollection();
	auto begin = objs->begin();
	const auto& end = objs->end();

	auto* graph = service.GetInteractionGraph();
	if (workerCount == 1)
	{
		auto group = graph->SelectGroup(begin, end, -1);
		_chunks.push(group);
		return 1;
	}

	// partitional by n
	//var offset = capacity >> 2;
	//for (int i = 0; i != workerCount; i++)
	//{
	//    _chunks.Push(new JobChunk { begin = 0, end = System.Math.Min(offset * i, capacity) });
	//}

	// partitional by small chunk
	//var head = 0;
	//var offset = Config.LENGTH_MIN_CHUNK;
	//while (head < capacity - 1)
	//{
	//    _chunks.Push(new JobChunk { begin = head, end = System.Math.Min((head += offset), capacity - 1) });
	//}

	int count = 0;
	int offset = _minJobchunkLength;
	while (true)
	{
		for (int i = 0; i != workerCount; i++)
		{
			auto res = graph->SelectGroup(begin, end, offset);
			if (!res.Success()) return res.error;

			if (res.val.GetCount() == 0) return Done(count);

			_chunks.push(res.val);
			count++;
		}

		offset <<= 1;
	}
}

void
JobPartitioner::Reset()
{
	_chunks.clear();
}
