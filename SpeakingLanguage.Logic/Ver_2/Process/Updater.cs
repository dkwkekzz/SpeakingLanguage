using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SpeakingLanguage.Logic.Process
{
    internal sealed class Updater
    {
        private readonly IJobContext _jobctx;
        private readonly int _id;

        public Updater(IJobContext ctx, int id)
        {
            _jobctx = ctx;
            _id = id;
        }

        public void Run()
        {
            Task.Factory.StartNew(() =>
            {
                Library.Tracer.Write($"[Updater] start run!");

                ref var service = ref EventManager.Instance.Service;
                var sync = _jobctx.SyncHandle;
                sync.SignalCompleted();

                var currentFrame = sync.Frame;
                var jobIter = _jobctx.JobPartitioner;
                var token = _jobctx.Token;
                while (!token.IsCancellationRequested)
                {
                    //Library.Tracer.Write($"[Updater] waiting: {_id.ToString()}");
                    if (!_spinUntilNextFrame(sync, currentFrame, ref token))
                        continue;

                    try
                    {
                        currentFrame = sync.Frame;

                        while (jobIter.MoveNext())
                        {
                            var groupIter = jobIter.Current;
                            if (groupIter.IsEmpty)
                                continue;

                            Interactor.Execute(ref service, ref groupIter);
                        }
                    }
                    catch (KeyNotFoundException e) { Library.Tracer.Error($"[Updater][critical]: {e.Message}/{e.StackTrace}"); }
                    catch (ArgumentException e) { Library.Tracer.Error($"[Updater][critical]: {e.Message}/{e.StackTrace}"); }
                    finally
                    {
                        //Library.Tracer.Write($"[Updater] completed: {_id.ToString()}");
                        if (0 < sync.SignalCompleted())
                            Thread.Yield();
                    }
                }

                Library.Tracer.Write($"[Updater] exit updater: {_id.ToString()}");
            });
        }

        private static bool _spinUntilNextFrame(SyncHandle sync, int currentFrame, ref CancellationToken token)
        {
            SpinWait spinner = new SpinWait();
            while (currentFrame >= sync.Frame)
            {
                spinner.SpinOnce();

                if (token.IsCancellationRequested)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool _spinUntilCompleted(SyncHandle sync, ref CancellationToken token)
        {
            SpinWait spinner = new SpinWait();
            while (!sync.Completed)
            {
                spinner.SpinOnce();

                if (token.IsCancellationRequested)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
