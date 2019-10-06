#pragma once
#include "stdafx.h"

namespace SpeakingLanguage
{
	class timer {
		typedef std::chrono::high_resolution_clock high_resolution_clock;
		typedef std::chrono::milliseconds milliseconds;
	public:
		explicit timer(bool run = false)
		{
			if (run)
				Reset();
		}
		void Reset()
		{
			_start = high_resolution_clock::now();
		}
		milliseconds Elapsed() const
		{
			return std::chrono::duration_cast<milliseconds>(high_resolution_clock::now() - _start);
		}
		template <typename T, typename Traits>
		friend std::basic_ostream<T, Traits>& operator<<(std::basic_ostream<T, Traits>& out, const timer& timer)
		{
			return out << timer.Elapsed().count();
		}
	private:
		high_resolution_clock::time_point _start;
	};
}