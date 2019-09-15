#include "stdafx.h"
#include "Notifier.h"
#include "JobContext.h"
#include "Updater.h"
#include "Utils/tracer.h"

using namespace SpeakingLanguage::Core::Process;

struct Notifier::Impl
{
	JobContext ctx;
	std::vector<Updater> workers;

	Impl(const StartInfo&);
};

Notifier::Impl::Impl(const StartInfo& info) : ctx(info)
{
	for (int i = 0; i != info.default_workercount; i++) workers.emplace_back(&ctx, i);
}

Notifier::Notifier(const StartInfo& info) : _pImpl(std::make_unique<Notifier::Impl>(info))
{
}

Notifier::~Notifier()
{
	_pImpl->ctx.tokenSource.SetValue(1);
}

void
Notifier::Awake()
{
	auto& workers = _pImpl->workers;
	for (auto it = workers.begin(); it != workers.end(); ++it) (*it).Run();
	_pImpl->ctx.sync.WaitForComplete();

	_pImpl->ctx.tokenSource.SetValue(0);
}

void
Notifier::Signal()
{
	const int workerCount = _pImpl->workers.size();
	if (_pImpl->ctx.partitioner.CollectJob(workerCount) == 0)
		return;

	auto& sync = _pImpl->ctx.sync;
	sync.SignalWorking();
	Utils::tracer::Log("signal update!");

	sync.WaitForComplete();
	Utils::tracer::Log("complete update!");
}

void
Notifier::Stop()
{
	_pImpl->ctx.tokenSource.SetValue(1);
}



//internal sealed class Notifier : IProcessor, IJobContext
//{
//	private readonly SyncHandle _syncHandle;
//	private readonly JobPartitioner _partitioner;
//
//	private readonly CancellationTokenSource _cts;
//	private readonly List<Updater> _lstUpdater;
//
//	public SyncHandle SyncHandle = > _syncHandle;
//	public JobPartitioner JobPartitioner = > _partitioner;
//	public CancellationToken Token = > _cts.Token;
//
//	public Notifier(ref StartInfo info)
//	{
//		_syncHandle = new SyncHandle(info.default_workercount);
//		_partitioner = new JobPartitioner(info.default_jobchunklength);
//		_cts = new CancellationTokenSource();
//		_lstUpdater = new List<Updater>(info.default_workercount);
//		for (int i = 0; i < info.default_workercount; i++) _lstUpdater.Add(new Updater(this, i));
//	}
//
//	public void Awake()
//	{
//		var updaterCount = _lstUpdater.Count;
//		for (int i = 0; i != updaterCount; i++) _lstUpdater[i].Run();
//		_syncHandle.WaitForComplete();
//
//		Library.Tracer.Write($"[Notifier] start run!");
//	}
//
//	public void Dispose()
//	{
//		_syncHandle.Dispose();
//		_cts.Cancel();
//		_cts.Dispose();
//	}
//
//	public void Signal(ref Service service)
//	{
//		Library.Tracer.Write($"[Notifier] start signal: {service.ElapsedTick.ToString()}");
//
//		var updaterCount = _lstUpdater.Count;
//		if (_partitioner.CollectJob(ref service, updaterCount) > 0)
//		{
//			Library.Tracer.Write($"[Notifier] send signal: {service.ElapsedTick.ToString()}");
//			_syncHandle.SignalWorking();
//
//			// 여기에 job을 넣을 수 있을까?
//
//			_syncHandle.WaitForComplete();
//		}
//
//		Library.Tracer.Write($"[Notifier] exit signal: {service.ElapsedTick.ToString()}");
//	}
//}