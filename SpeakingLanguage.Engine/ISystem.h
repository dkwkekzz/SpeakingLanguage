#pragma once
#include "Result.h"
#include "SyncPair.h"
#include "InteractPair.h"

namespace SpeakingLanguage
{
	class ISystem
	{
	public:
		using TAction = void(__stdcall*)(int);

		virtual ~ISystem() {}
		virtual Result<void> Insert(const InteractPair& itr) = 0;

		inline SyncPair GetSync() const { return _sync; }

	protected:
		ISystem(const SyncPair& sync) : _sync(sync) {}

	private:
		SyncPair _sync;

	};
}
