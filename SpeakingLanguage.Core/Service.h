#pragma once
#include "StartInfo.h"

namespace SpeakingLanguage { namespace Core
{
	class Service
	{
	public:
		explicit Service(const StartInfo&);
		~Service();


	private:
		slObjectCollection _objs;
	};

} }
