#pragma once
#include <iostream>
#include <ostream>

namespace SpeakingLanguage 
{
	class tracer
	{
	public:
		template<typename TMsg>
		static void log(TMsg&& msg)
		{
#ifdef TRACE_STD
			std::cout << "[log] " << __FUNCTION__ << '(' << __LINE__ << ") " << msg << std::endl;
#endif
#ifdef TRACE_OUTPUT
			std::clog << "[log] " << __FUNCTION__ << '(' << __LINE__ << ") " << msg << std::endl;
#endif
		}

		template<typename TMsg>
		static void error(TMsg&& msg)
		{
#ifdef TRACE_STD
			std::cout << "[error] " << __FUNCTION__ << '(' << __LINE__ << ") " << msg << std::endl;
#endif
#ifdef TRACE_OUTPUT
			std::clog << "[error] " << __FUNCTION__ << '(' << __LINE__ << ") " << msg << std::endl;
#endif
		}

	private:
		tracer() {}

	};
}
