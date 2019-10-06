#pragma once
#include "StartInfo.h"

namespace SpeakingLanguage { namespace Core
{
	struct WorkContext;

	class Notifier
	{
	public:
		explicit Notifier(const StartInfo&);
		~Notifier();

		void Awake();
		void Signal(Service& service);
		void Stop();

	private:
		struct Impl;
		std::unique_ptr<Impl> _pImpl;
		std::shared_ptr<WorkContext> _pCtx;
	};
} }
