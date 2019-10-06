#include "stdafx.h"
#include "Worker.h"
#include "WorkContext.h"
#include "tracer.h"
#include "spinwait.h"

using namespace SpeakingLanguage;

static bool _spinUntilNextFrame(const SyncHandle& sync, int currentFrame, const CancelTokenSource::Token& token);
static bool _spinUntilCompleted(const SyncHandle& sync, const CancelTokenSource::Token& token);

bool _spinUntilNextFrame(const SyncHandle& sync, int currentFrame, const CancelTokenSource::Token& token)
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

bool _spinUntilCompleted(const SyncHandle& sync, const CancelTokenSource::Token& token)
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


Worker::Worker(int id) :_id(id)
{
}

Worker::Worker(Worker&& other) : _worker(std::move(other._worker)), _id(other._id)
{
}

Worker::~Worker()
{
}

void
Worker::Run(std::shared_ptr<WorkContext> ctx)
{
	if (_worker != nullptr) return;

	_worker = std::make_unique<std::thread>([ctx = std::move(ctx)]()
	{
		auto& sync = ctx->sync;
		sync.SignalCompleted();

		const auto& token = ctx->tokenSource.GetToken();
		while (!token.IsCancel())
		{
			const int currentFrame = sync.GetFrame();
			if (!_spinUntilNextFrame(sync, currentFrame, token))
					continue;

			tracer::log("execute update...");
			// execute job

			sync.SignalCompleted();

			// sleep until nextFrameEstimatedTick
			std::this_thread::sleep_for(60ms);
		}
	});
}
