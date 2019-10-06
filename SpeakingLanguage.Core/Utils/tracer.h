#pragma once

namespace SpeakingLanguage 
{
	class tracer
	{
	public:
		static void Log(const std::string&);
		static void Log(std::string&&);

	private:
		tracer() {}
	};
}
