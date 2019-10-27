#pragma once
#include "stdafx.h"

namespace SpeakingLanguage {
	namespace Core {

		struct slChunk;

		class IAllocator
		{
		public:
			virtual ~IAllocator() {}
			virtual slChunk* Alloc(int size) = 0;
		};
	}
}