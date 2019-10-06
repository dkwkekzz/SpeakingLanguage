#pragma once
#include "ISystem.h"
#include "ActionSource.h"

namespace SpeakingLanguage
{
	class StateCollection
	{
	public:
		template<typename T>
		static std::vector<T>* GetContainer()
		{
			static std::vector<T> container;
			return &container;
		}

	};
}
namespace SpeakingLanguage
{
	class SimpleSystem : public ISystem
	{
	public:
		explicit SimpleSystem(const ActionSource& action);
		virtual ~SimpleSystem();

		virtual Result<void> Insert(const InteractPair& itr) override;

	private:
		TAction _action;

	};
}