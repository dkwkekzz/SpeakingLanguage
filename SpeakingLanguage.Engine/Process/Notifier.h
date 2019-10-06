#pragma once
#include "Locator.h"
#include "StartInfo.h"

namespace SpeakingLanguage 
{
	class Service;
	struct WorkContext;

	class Notifier : public IProcessor
	{
	public:
		explicit Notifier(const StartInfo& info);
		~Notifier();

		void Awake();
		void Signal();
		void Stop();

	private:
		struct Impl;
		std::unique_ptr<Impl> _impl;
		std::shared_ptr<WorkContext> _ctx;
	};
} 
