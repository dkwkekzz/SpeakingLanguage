#include "stdafx.h"
#include "Notifier.h"
#include "Worker.h"
#include "WorkContext.h"
#include "tracer.h"

using namespace SpeakingLanguage;

struct Notifier::Impl
{
	std::vector<Worker> workers;

	Impl(const StartInfo&);
};

Notifier::Impl::Impl(const StartInfo& info)
{
	for (int i = 0; i != info.default_workercount; i++) workers.emplace_back(i);
}

Notifier::Notifier(const StartInfo& info) : _impl(std::make_unique<Notifier::Impl>(info)), _ctx(std::make_shared<WorkContext>(info))
{
}

Notifier::~Notifier()
{
	_ctx->tokenSource.SetValue(1);
}

void
Notifier::Awake()
{
	auto& workers = _impl->workers;
	for (auto it = workers.begin(); it != workers.end(); ++it) (*it).Run(_ctx);
	
	_ctx->sync.WaitForComplete();
	_ctx->tokenSource.SetValue(0);
}

void
Notifier::Signal()
{
	auto& sync = _ctx->sync;
	sync.SignalWorking();
	tracer::log("signal update!");

	sync.WaitForComplete();
	tracer::log("complete update!");
}

void
Notifier::Stop()
{
	_ctx->tokenSource.SetValue(1);
}
