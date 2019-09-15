#pragma once

namespace SpeakingLanguage {
	namespace Utils {

		class tracer
		{
		public:
			static void Log(const std::string&);
			static void Log(std::string&&);

		private:
			tracer() {}
		};
	}
}
