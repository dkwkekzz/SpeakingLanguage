#pragma once

namespace SpeakingLanguage {
	namespace Core {
		namespace State {

			struct IState {};

			struct Basic
			{
				int lifeCycle;
				int position;
				int detection;
			};
		}
	}
}