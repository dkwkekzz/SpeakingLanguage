using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SpeakingLanguage.Logic.Process
{
    internal sealed class Notifier : IProcessor, IJobContext
    {
        private readonly SyncHandle _syncHandle;
        private readonly JobPartitioner _jobIter;

        private readonly CancellationTokenSource _cts;
        private readonly List<Updater> _lstUpdater;
        
        public SyncHandle SyncHandle => _syncHandle;
        public JobPartitioner JobPartitioner => _jobIter;
        public CancellationToken Token => _cts.Token;

        public Notifier(ref StartInfo info)
        {
            _syncHandle = new SyncHandle(info.default_workercount);
            _jobIter = new JobPartitioner(info.default_jobchunklength);
            _cts = new CancellationTokenSource();
            _lstUpdater = new List<Updater>(info.default_workercount);
            for (int i = 0; i != info.default_workercount; i++) _lstUpdater.Add(new Updater(this, i));
        }

        public void Awake()
        {
            for (int i = 0; i != _lstUpdater.Count; i++) _lstUpdater[i].Run();
            _syncHandle.WaitForComplete();

            Library.Tracer.Write($"[Notifier] start run!");
        }

        public void Dispose()
        {
            _syncHandle.Dispose();
            _cts.Cancel();
            _cts.Dispose();
        }

        public void Signal(ref Service service)
        {
            if (_jobIter.CollectJob(ref service, _lstUpdater.Count) > 0)
            {
                Library.Tracer.Write($"[Notifier] send signal!");
                _syncHandle.SignalWorking();

                // 여기에 job을 넣을 수 있을까?
                while (_jobIter.MoveNext())
                {
                    var groupIter = _jobIter.Current;
                    Interactor.Execute(ref service, ref groupIter);
                }

                Library.Tracer.Write($"[Notifier] waiting...");
                _syncHandle.WaitForComplete();
            }
        }
    }
}
