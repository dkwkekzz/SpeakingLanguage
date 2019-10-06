// Service.cpp
#include "stdafx.h"
#include "Service.h"
#include "slObjectCollection.h"
#include "InteractionGraph.h"
#include "JobCollection.h"

using namespace SpeakingLanguage::Core;

Service::Service(const StartInfo& info) : 
	_pObjs(std::make_unique<slObjectCollection>(info.default_objectcount)),
	_pGraph(std::make_unique<InteractionGraph>(info.default_objectcount)),
	_pJob(std::make_unique<JobCollection>())
{
}

Service::~Service()
{
}