#pragma once
#include "CountdownEvent.h"

namespace SpeakingLanguage { namespace Core { namespace Process
{
	class SyncHandle
	{
	public:
		explicit SyncHandle(int workerCount) : _frame(0), _eventHandle(workerCount) {}
		~SyncHandle() {}

		inline int GetFrame() const { return _frame.load(); }
		inline bool Completed() const { return _eventHandle.GetCurrentCount() == 0; }

		inline void SignalCompleted()
		{
			_eventHandle.Signal();
		}

		inline void SignalWorking()
		{
			_eventHandle.Reset();
			_frame.fetch_add(1);
		}

		inline void WaitForComplete()
		{
			_eventHandle.Wait();
		}

	private:
		std::atomic<int> _frame;
		CountdownEvent _eventHandle;

	};
} 
} }


//internal sealed class SyncHandle : IDisposable
//{
//	private CountdownEvent _completeHandle;
//	private int _frame;
//
//	public int Frame = > Volatile.Read(ref _frame);
//	public int WorkerCount = > _completeHandle.InitialCount;
//	public bool Completed = > _completeHandle.IsSet;
//
//	public SyncHandle(int workerCount)
//	{
//		_completeHandle = new CountdownEvent(workerCount);
//		_frame = 0;
//	}
//
//	public void Dispose()
//	{
//		_completeHandle.Dispose();
//		_frame = -1;
//	}
//
//	public void Reset(int workerCount)
//	{
//		_completeHandle.Reset(workerCount);
//	}
//
//	public int SignalCompleted()
//	{
//		_completeHandle.Signal();
//		return _completeHandle.CurrentCount;
//	}
//
//	public void SignalWorking()
//	{
//		_completeHandle.Reset();
//		Interlocked.Increment(ref _frame);
//	}
//
//	public void WaitForComplete()
//	{
//		_completeHandle.Wait();
//	}
//}