#pragma once

namespace SpeakingLanguage { namespace Core { namespace Process
{
	class JobPartitioner
	{
	public:
		using JobType = std::pair<int, int>;

		explicit JobPartitioner(int);

		inline bool HasNext() { return _chunks.unsafe_size() > 0; }
		inline const JobType& GetCurrent()
		{
			JobType job;
			if (!_chunks.try_pop(job)) return job;
			return job;
		}

		int CollectJob(int);
		void Reset();

	private:
		Concurrency::concurrent_queue<JobType> _chunks;
		const int _minJobchunkLength;
	};
} 
} }


//
//namespace SpeakingLanguage.Logic.Process
//{
//	internal sealed class JobPartitioner : IEnumerator<Data.InteractGroup>
//	{
//		private readonly ConcurrentStack<Data.InteractGroup> _chunks = new ConcurrentStack<Data.InteractGroup>();
//		private readonly int _minJobchunkLength;
//
//		public JobPartitioner(int jobchunkLength)
//		{
//			_minJobchunkLength = jobchunkLength;
//		}
//
//		public Data.InteractGroup Current
//		{
//			get
//			{
//				Data.InteractGroup group;
//				if (!_chunks.TryPop(out group))
//					return default(Data.InteractGroup);
//				return group;
//			}
//		}
//		object IEnumerator.Current = > Current;
//
//		public void Dispose()
//		{
//			Reset();
//		}
//
//		public int CollectJob(ref Service service, int workerCount)
//		{
//			var objIter = service.colObj.GetEnumerator();
//			var count = 0;
//			if (workerCount == 1)
//			{
//				if (!service.itrGraph.TryGetInteractGroup(ref objIter, -1, out Data.InteractGroup group))
//					return count;
//
//				_chunks.Push(group);
//				return ++count;
//			}
//
//			var offset = _minJobchunkLength;
//			while (true)
//			{
//				for (int i = 0; i != workerCount; i++)
//				{
//					if (!service.itrGraph.TryGetInteractGroup(ref objIter, offset, out Data.InteractGroup group))
//						return count;
//
//					_chunks.Push(group);
//					++count;
//				}
//
//				offset <<= 1;
//			}
//		}
//
//		public bool MoveNext()
//		{
//			return _chunks.Count > 0;
//		}
//
//		public void Reset()
//		{
//			if (_chunks.Count > 0)
//				_chunks.Clear();
//		}
//	}
//}
