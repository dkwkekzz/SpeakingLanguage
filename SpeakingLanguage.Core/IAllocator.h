#pragma once
#include "stdafx.h"

namespace SpeakingLanguage {
	namespace Core {

		struct slChunk;

		class IAllocator
		{
		public:
			virtual ~IAllocator() = 0;
			virtual slChunk* Alloc(int size) = 0;
		};
	}
}