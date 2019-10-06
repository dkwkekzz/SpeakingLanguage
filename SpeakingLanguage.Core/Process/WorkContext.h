#pragma once
#include "StartInfo.h"
#include "SyncHandle.h"
#include "JobPartitioner.h"
#include "CancelTokenSource.h"

namespace SpeakingLanguage { namespace Core
{
	struct WorkContext
	{
		SyncHandle sync;
		JobPartitioner partitioner;
		CancelTokenSource tokenSource;

		explicit WorkContext(const StartInfo& info) :
			sync(info.default_workercount),
			partitioner(info.default_jobchunklength)
		{
		}
	};
} 
}
