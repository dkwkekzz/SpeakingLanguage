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
			inline void Provide(T* service) { _service = std::make_unique<T>(); }	// shared로 변경... 공유될수있으므로

		private:
			std::unique_ptr<T> _service;

		};

		static Provider<IInjector<Interaction>> InteractionInjector;
		static Provider<IInjector<ActionSource>> ActionInjector;

	private:
		Locator();
		~Locator();
	};

}
