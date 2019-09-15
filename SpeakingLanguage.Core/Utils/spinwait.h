#pragma once
#include "stdafx.h"

namespace SpeakingLanguage {
	namespace Utils {

		constexpr unsigned int YIELD_ITERATION = 30; // yeild after 30 iterations
		constexpr unsigned int MAX_SLEEP_ITERATION = 40;

		class spinwait
		{
		public:
			inline bool HasThreasholdReached() { return (m_iterations >= YIELD_ITERATION); }
			void SpinOnce();

		private:
			unsigned int m_iterations{ 0 };
		};

		void
		spinwait::SpinOnce()
		{
			if (HasThreasholdReached())
			{
				if (m_iterations + YIELD_ITERATION >= MAX_SLEEP_ITERATION)
					Sleep(0);

				if (m_iterations >= YIELD_ITERATION && m_iterations < MAX_SLEEP_ITERATION)
				{
					m_iterations = 0;
					std::this_thread::yield();
				}
			}
			m_iterations++;

			// Yield processor on multi-processor but if on single processor then give other thread the CPU
			std::this_thread::yield();
		}
	}
}