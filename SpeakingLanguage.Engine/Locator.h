#pragma once
#include "Result.h"
#include "Interaction.h"
#include "ActionSource.h"

namespace SpeakingLanguage
{
	template<typename T>
	class IInjector
	{
	public:
		virtual ~IInjector() {}
		virtual Result<void> Insert(const T&) = 0;
	};

	class IProcessor
	{
	public:
		virtual ~IProcessor() {}
		virtual void Awake() = 0;
		virtual void Signal() = 0;
		virtual void Stop() = 0;
	};

	template<typename T>
	class ICollector
	{
	public:
		virtual ~ICollector() {}
		virtual Result<void> Add(std::shared_ptr<T> sp) = 0;
	};

	class Locator
	{
	public:
		template<typename T>
		class Provider
		{
		public:
			inline T* Get() { return _service.get(); }
			inline void Provide(T* service) { _service.reset(service); }

		private:
			std::unique_ptr<T> _service;

		};

		static Provider<IInjector<Interaction>> InteractionInjector;
		static Provider<IInjector<ActionSource>> ActionInjector;
		static Provider<IProcessor> Processor;
		static Provider<ICollector<ISystem>> SystemCollector;

	private:
		Locator();
		~Locator();
	};

}
