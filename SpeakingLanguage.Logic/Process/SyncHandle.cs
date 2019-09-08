using System;
using System.Collections.Generic;
using System.Threading;

namespace SpeakingLanguage.Logic.Process
{
    internal sealed class SyncHandle : IDisposable
    {
        private CountdownEvent _completeHandle;
        private int _frame;

        public int Frame => Volatile.Read(ref _frame);
        public int WorkerCount => _completeHandle.InitialCount;
        public bool Completed => _completeHandle.IsSet;

        public SyncHandle(int workerCount)
        {
            _completeHandle = new CountdownEvent(workerCount);
            _frame = 0;
        }
        
        public void Dispose()
        {
            _completeHandle.Dispose();
            _frame = -1;
        }

        public void Reset(int workerCount)
        {
            _completeHandle.Reset(workerCount);
        }

        public int SignalCompleted()
        {
            _completeHandle.Signal();
            return _completeHandle.CurrentCount;
        }

        public void SignalWorking()
        {
            _completeHandle.Reset();
            Interlocked.Increment(ref _frame);
        }

        public void WaitForComplete()
        {
            _completeHandle.Wait();
        }
    }
}
