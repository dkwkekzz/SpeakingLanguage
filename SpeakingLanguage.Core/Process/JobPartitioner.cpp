#include "stdafx.h"
#include "JobPartitioner.h"

using namespace SpeakingLanguage::Core::Process;

JobPartitioner::JobPartitioner(int jobchunkLength) : _minJobchunkLength(jobchunkLength)
{
}

int
JobPartitioner::CollectJob(int workerCount)
{
	return 1;
}

void
JobPartitioner::Reset()
{
}
