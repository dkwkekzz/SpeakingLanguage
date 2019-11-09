#pragma once
#include "slObject.h"
#include "stdafx.h"

namespace SpeakingLanguage
{
	enum class LogicalConnective : unsigned char
	{
		Always,
		And,
		Or,
	};

	struct Attack
	{
		int value;
	};

	class AttackStateSystem
	{
	public:
		inline void Insert(LogicalConnective lc, std::function<bool(Attack&)>&& cond) { _lawConds.emplace_back(lc, cond); }

	private:
		std::vector<std::pair<LogicalConnective, std::function<bool(Attack&)> > > _lawConds;
	};

	struct Defence
	{
		int value;
	};

	struct Hp
	{
		int value;
	};

	struct Law
	{

		void onInject(AttackStateSystem& system)
		{
			// if cond1 & cond2 == true, execute res1.
			auto cond1 = [](Attack& att) { return att.value > 5; };
			system.Insert(LogicalConnective::Always, cond1);

			auto cond2 = [](Defence& def) { return def.value > 5; };
			system.Insert(LogicalConnective::And, cond2);

			auto res1 = [](Hp& hp) { hp.value += 100; };
		}

		void onInteract(slObject& subject, slObject& target)
		{

		}
	};

	struct Event
	{

	};

	struct Interaction
	{
		slObject::THandle subject;
		slObject::THandle target;
	};

	struct slState
	{
		using THandle = int;
	};
}