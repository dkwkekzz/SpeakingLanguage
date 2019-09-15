#pragma once
#include "stdafx.h"

namespace SpeakingLanguage { namespace Core { namespace Process 
{
	class CountdownEvent
	{
	public:
		CountdownEvent(int count);
		~CountdownEvent();

		inline int GetCurrentCount() const { return _currentCount.load(); }
		inline void Signal() { _currentCount.fetch_sub(1); }
		inline void Reset() { _currentCount.store(_initCount); }
		void Wait();

	private:
		int _initCount;
		std::atomic<int> _currentCount;
	};
} } }