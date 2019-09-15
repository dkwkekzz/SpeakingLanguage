#include "stdafx.h"
#include "Updater.h"
#include "JobContext.h"
#include "Utils/tracer.h"
#include "Utils/spinwait.h"

using namespace SpeakingLanguage::Core::Process;

struct Updater::Helper
{
	static bool _spinUntilNextFrame(const SyncHandle& sync, int currentFrame, const CancelTokenSource::Token& token);
	static bool _spinUntilCompleted(const SyncHandle& sync, const CancelTokenSource::Token& token);
};

bool
Updater::Helper::_spinUntilNextFrame(const SyncHandle& sync, int currentFrame, const CancelTokenSource::Token& token)
{
	Utils::spinwait spinner;
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
	Utils::spinwait spinner;
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


Updater::Updater(JobContext* ctx, int id) : _pCtx(ctx), _id(id)
{
}

Updater::Updater(Updater&& other) : _pWorker(std::move(other._pWorker)), _pCtx(other._pCtx), _id(other._id)
{
}

Updater::~Updater()
{
}

void
Updater::Run()
{
	if (_pWorker != nullptr) return;

	_pWorker = std::make_unique<std::thread>([this]()
	{
		auto& sync = _pCtx->sync;
		sync.SignalCompleted();

		auto& jobIter = _pCtx->partitioner;

		const auto& token = _pCtx->tokenSource.GetToken();
		while (!token.IsCancel())
		{
			const int currentFrame = sync.GetFrame();
			if (!Updater::Helper::_spinUntilNextFrame(sync, currentFrame, token))
					continue;

			Utils::tracer::Log("execute update...");

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