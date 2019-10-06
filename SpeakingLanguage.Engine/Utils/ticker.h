#pragma once
#include "stdafx.h"

namespace SpeakingLanguage 
{
	class ticker
	{
	public:
		static inline long long GetTicks()
		{
			auto now = std::chrono::system_clock::now();
			auto duration = now.time_since_epoch();
			return std::chrono::duration_cast<std::chrono::milliseconds>(duration).count();
		}
	};
}