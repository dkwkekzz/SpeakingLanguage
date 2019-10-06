#pragma once
#include "StartInfo.h"
#include "SyncHandle.h"
#include "CancelTokenSource.h"

namespace SpeakingLanguage 
{
	struct WorkContext
	{
		SyncHandle sync;
		CancelTokenSource tokenSource;

		explicit WorkContext(const StartInfo& info) : sync(info.default_workercount)
		{
		}
	};
} 
