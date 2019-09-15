#pragma once

namespace SpeakingLanguage {
	namespace Core {

		struct slChunk
		{
			int typeHandle;
			int length;

			template<typename T>
			T* Get() { return (T*)((uint64_t)this + sizeof(slChunk)); }

			inline slChunk* GetNext() { return (slChunk*)((uint64_t)this + sizeof(slChunk) + length); }

			template<typename T>
			static slChunk* GetChunk(T* ptr) { return (slChunk*)((uint64_t)ptr - sizeof(slChunk)); }
		};
	}
}