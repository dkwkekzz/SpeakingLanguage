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
            for (int i = 0; i < info.default_workercount; i++) _lstUpdater.Add(new Updater(this, i));
        }

        public void Awake()
        {
            var updaterCount = _lstUpdater.Count;
            for (int i = 0; i != updaterCount; i++) _lstUpdater[i].Run();
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
            Library.Tracer.Write($"[Notifier] start signal: {service.ElapsedTick.ToString()}");

            var updaterCount = _lstUpdater.Count;
            if (_jobIter.CollectJob(ref service, updaterCount) > 0)
            {
                Library.Tracer.Write($"[Notifier] send signal: {service.ElapsedTick.ToString()}");
                _syncHandle.SignalWorking();

                // 여기에 job을 넣을 수 있을까?

                _syncHandle.WaitForComplete();
            }

            Library.Tracer.Write($"[Notifier] exit signal: {service.ElapsedTick.ToString()}");
        }
    }
}
