#pragma once
#include "Locator.h"

namespace SpeakingLanguage
{
	class SystemCollection : public ICollector<ISystem>
	{
	public:
		explicit SystemCollection();
		~SystemCollection();

		virtual Result<void> Add(std::shared_ptr<ISystem> system) override;

	private:
		std::vector<std::shared_ptr<ISystem>> _systems;
	};
}
