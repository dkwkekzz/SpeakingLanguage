﻿using System;
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

                var jobIter = _jobctx.JobPartitioner;
                var token = _jobctx.Token;
                while (!token.IsCancellationRequested)
                {
                    Library.Tracer.Write($"[Updater] waiting: {_id.ToString()}");
                    sync.WaitForWork();

                    try
                    {
                        while (jobIter.MoveNext())
                        {
                            var groupIter = jobIter.Current;
                            Interactor.Execute(ref service, ref groupIter);
                        }

                        Thread.Yield();
                    }
                    catch (NotImplementedException e) { Library.Tracer.Error($"[Updater] critical: {e.Message}/{e.StackTrace}"); }
                    catch (KeyNotFoundException e) { Library.Tracer.Error($"[Updater] critical: {e.Message}/{e.StackTrace}"); }
                    catch (ArgumentException e) { Library.Tracer.Error($"[Updater] critical: {e.Message}/{e.StackTrace}"); }
                    catch (Exception e) { Library.Tracer.Error($"[Updater] critical: {e.Message}/{e.StackTrace}"); }
                    finally
                    {
                        sync.SignalCompleted();
                        Library.Tracer.Write($"[Updater] completed: {_id.ToString()}");
                    }
                }
            });
        }
    }
}
