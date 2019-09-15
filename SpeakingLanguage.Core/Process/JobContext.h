#pragma once
#include "StartInfo.h"
#include "SyncHandle.h"
#include "JobPartitioner.h"
#include "CancelTokenSource.h"

namespace SpeakingLanguage { namespace Core { namespace Process
{
	struct JobContext
	{
		SyncHandle sync;
		JobPartitioner partitioner;
		CancelTokenSource tokenSource;

		explicit JobContext(const StartInfo& info) :
			sync(info.default_workercount),
			partitioner(info.default_jobchunklength)
		{
		}
	};
} 
} }
