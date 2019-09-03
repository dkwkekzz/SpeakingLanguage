using System;
using System.Collections.Generic;
using System.Threading;

namespace SpeakingLanguage.Logic.Process
{
    internal sealed class SyncHandle : IDisposable
    {
        private CountdownEvent _completeHandle;
        private ManualResetEventSlim _waitHandle;

        public int WorkerCount => _completeHandle.InitialCount;
        public bool IsCompleted => _completeHandle.IsSet;

        public SyncHandle(int workerCount)
        {
            _completeHandle = new CountdownEvent(workerCount);
            _waitHandle = new ManualResetEventSlim(false);
        }
        
        public void Dispose()
        {
            _completeHandle.Dispose();
            _waitHandle.Dispose();
        }

        public void Reset(int workerCount)
        {
            _completeHandle.Reset(workerCount);
            _waitHandle.Reset();
        }

        public void SignalCompleted()
        {
            _waitHandle.Reset();
            if (!_completeHandle.IsSet)
                _completeHandle.Signal();
        }

        public void SignalWorking()
        {
            _completeHandle.Reset();
            _waitHandle.Set();
        }

        public void WaitForComplete()
        {
            if (!_completeHandle.IsSet)
                _completeHandle.Wait();
        }

        public void WaitForWork()
        {
            _waitHandle.Wait();
        }
    }
}
