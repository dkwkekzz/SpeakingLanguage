#pragma once

namespace SpeakingLanguage { namespace Core { namespace Process
{
	struct JobContext;

	class Updater
	{
	public:
		explicit Updater(JobContext*, int);
		Updater(Updater&& other);
		~Updater();

		void Run();

	private:
		struct Helper;

		std::unique_ptr<std::thread> _pWorker;
		JobContext* _pCtx;
		const int _id; 
	};
} 
} }

