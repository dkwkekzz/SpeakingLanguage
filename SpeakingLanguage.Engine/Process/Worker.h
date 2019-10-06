#pragma once

namespace SpeakingLanguage 
{
	struct WorkContext;

	class Worker
	{
	public:
		explicit Worker(int);
		Worker(Worker&& other);
		~Worker();

		void Run(std::shared_ptr<WorkContext> ctx);

	private:
		std::unique_ptr<std::thread> _worker;
		const int _id; 
	};
} 

