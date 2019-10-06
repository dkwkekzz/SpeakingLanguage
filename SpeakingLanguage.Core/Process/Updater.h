#pragma once

namespace SpeakingLanguage { namespace Core
{
	struct WorkContext;

	class Updater
	{
	public:
		explicit Updater(int);
		Updater(Updater&& other);
		~Updater();

		void Run(std::shared_ptr<WorkContext> ctx);

	private:
		struct Helper;

		std::unique_ptr<std::thread> _pWorker;
		const int _id; 
	};
} }

