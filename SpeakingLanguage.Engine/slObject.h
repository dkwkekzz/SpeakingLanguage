#pragma once
#include <bitset>

namespace SpeakingLanguage
{
	struct slObject
	{	// ´ë»ó
		using THandle = int;
		using TSync = int;

		enum class ESort : unsigned char
		{
			Empty = 0,
			Static,
			Dynamic,
		};

		THandle handle;
		ESort sort;
		TSync sync;
	};
}