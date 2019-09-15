#pragma once
#include "StartInfo.h"

namespace SpeakingLanguage { namespace Core { namespace Process
{
	class Notifier
	{
	public:
		explicit Notifier(const StartInfo&);
		~Notifier();

		void Awake();
		void Signal();
		void Stop();

	private:
		struct Impl;
		std::unique_ptr<Impl> _pImpl;
	};
} 
} }
