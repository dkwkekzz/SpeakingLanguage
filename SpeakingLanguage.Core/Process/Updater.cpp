#include "stdafx.h"
#include "Updater.h"
#include "WorkContext.h"
#include "Utils/tracer.h"
#include "Utils/spinwait.h"

using namespace SpeakingLanguage::Core;

struct Updater::Helper
{
	static bool _spinUntilNextFrame(const SyncHandle& sync, int currentFrame, const CancelTokenSource::Token& token);
	static bool _spinUntilCompleted(const SyncHandle& sync, const CancelTokenSource::Token& token);
};

bool
Updater::Helper::_spinUntilNextFrame(const SyncHandle& sync, int currentFrame, const CancelTokenSource::Token& token)
{
	spinwait spinner;
	while (currentFrame >= sync.GetFrame())
	{
		spinner.SpinOnce();

		if (token.IsCancel())
		{
			return false;
		}
	}
	return true;
}

bool
Updater::Helper::_spinUntilCompleted(const SyncHandle& sync, const CancelTokenSource::Token& token)
{
	spinwait spinner;
	while (!sync.Completed())
	{
		spinner.SpinOnce();

		if (token.IsCancel())
		{
			return false;
		}
	}
	return true;
}


Updater::Updater(int id) :_id(id)
{
}

Updater::Updater(Updater&& other) : _pWorker(std::move(other._pWorker)), _id(other._id)
{
}

Updater::~Updater()
{
}

void
Updater::Run(std::shared_ptr<WorkContext> ctx)
{
	if (_pWorker != nullptr) return;

	_pWorker = std::make_unique<std::thread>([pCtx = std::move(ctx)]()
	{
		auto& sync = pCtx->sync;
		sync.SignalCompleted();

		auto& jobIter = pCtx->partitioner;

		const auto& token = pCtx->tokenSource.GetToken();
		while (!token.IsCancel())
		{
			const int currentFrame = sync.GetFrame();
			if (!Updater::Helper::_spinUntilNextFrame(sync, currentFrame, token))
					continue;

			tracer::Log("execute update...");
			while (jobIter.HasNext())
			{
				JobPartitioner::JobType job;
				if (!jobIter.TryGetCurrent(job)) continue;

				// execute job
			}

			sync.SignalCompleted();

			// sleep until nextFrameEstimatedTick
			std::this_thread::sleep_for(60ms);
		}
	});
}

//internal sealed class Updater
//{
//	private readonly IJobContext _jobctx;
//	private readonly int _id;
//
//	public Updater(IJobContext ctx, int id)
//	{
//		_jobctx = ctx;
//		_id = id;
//	}
//
//	public void Run()
//	{
//		Task.Factory.StartNew(() = >
//		{
//			Library.Tracer.Write($"[Updater] start run!");
//
//			ref var service = ref EventManager.Instance.Service;
//			var sync = _jobctx.SyncHandle;
//			sync.SignalCompleted();
//
//			var currentFrame = sync.Frame;
//			var jobIter = _jobctx.JobPartitioner;
//			var token = _jobctx.Token;
//			while (!token.IsCancellationRequested)
//			{
//				//Library.Tracer.Write($"[Updater] waiting: {_id.ToString()}");
//				if (!_spinUntilNextFrame(sync, currentFrame, ref token))
//					continue;
//
//				try
//				{
//					currentFrame = sync.Frame;
//
//					while (jobIter.MoveNext())
//					{
//						var groupIter = jobIter.Current;
//						if (groupIter.IsEmpty)
//							continue;
//
//						Interactor.Execute(ref service, ref groupIter);
//					}
//				}
//				catch (KeyNotFoundException e) { Library.Tracer.Error($"[Updater][critical]: {e.Message}/{e.StackTrace}"); }
//				catch (ArgumentException e) { Library.Tracer.Error($"[Updater][critical]: {e.Message}/{e.StackTrace}"); }
//				finally
//				{
//					//Library.Tracer.Write($"[Updater] completed: {_id.ToString()}");
//					if (0 < sync.SignalCompleted())
//						Thread.Yield();
//				}
//			}
//
//			Library.Tracer.Write($"[Updater] exit updater: {_id.ToString()}");
//		});
//	}
//
//	private static bool _spinUntilNextFrame(SyncHandle sync, int currentFrame, ref CancellationToken token)
//	{
//		SpinWait spinner = new SpinWait();
//		while (currentFrame >= sync.Frame)
//		{
//			spinner.SpinOnce();
//
//			if (token.IsCancellationRequested)
//			{
//				return false;
//			}
//		}
//		return true;
//	}
//
//	private static bool _spinUntilCompleted(SyncHandle sync, ref CancellationToken token)
//	{
//		SpinWait spinner = new SpinWait();
//		while (!sync.Completed)
//		{
//			spinner.SpinOnce();
//
//			if (token.IsCancellationRequested)
//			{
//				return false;
//			}
//		}
//		return true;
//	}
//}