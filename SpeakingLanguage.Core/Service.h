#pragma once
#include "StartInfo.h"

namespace SpeakingLanguage { namespace Core
{
	class slObjectCollection;
	class InteractionGraph;
	class JobCollection;

	class Service
	{
	public:
		explicit Service(const StartInfo&);
		~Service();

		inline slObjectCollection* GetObjectCollection() { return _pObjs.get(); }
		inline InteractionGraph* GetInteractionGraph() { return _pGraph.get(); }
		inline JobCollection* GetJobCollection() { return _pJob.get(); }

	private:
		std::unique_ptr<slObjectCollection> _pObjs;
		std::unique_ptr<InteractionGraph> _pGraph;
		std::unique_ptr<JobCollection> _pJob;
	};

} }
