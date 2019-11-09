#pragma once
#include "slObject.h"
#include "Result.h"

namespace SpeakingLanguage
{
	class Law
	{
	public:
		inline Result<void> Insert(const slObject::THandle handle) { _subjects.emplace_back(handle); return Done(); }

	private:
		std::vector<slObject::THandle> _subjects;
	};

	class Archetype
	{
	public:
		Result<void> Insert(const slObject::THandle handle);

	private:
		std::vector<std::shared_ptr<Law>> _affectedPossibilities;
	};

	Result<void> Archetype::Insert(const slObject::THandle handle)
	{
		for (auto& law : _affectedPossibilities)
		{
			auto ret = law->Insert(handle);
			if (!ret.Success()) return ret;
		}
		return Done();
	}

	class StateSystem
	{
	public:
		inline Result<void> Insert(const slObject::THandle handle) { _subjectList.emplace_back(handle); }

	private:
		std::vector<slObject::THandle> _subjectList;
		std::vector<std::shared_ptr<Law>> _lawList;

	};

	class SystemCollection
	{
	public:
		static constexpr int MAX_SYSTEM_COUNT = 256;

		inline Result<void> Insert(const slObject::THandle handle) { _subjects.emplace_back(handle); }

		inline Result<void> Insert(const slObject::THandle handle, const int systemIdx)
		{
			if (_systems.size() <= systemIdx) return Error::OutOfRange;
			return _systems[systemIdx].Insert(handle);
		}

	private:
		std::vector<slObject::THandle> _subjects;
		std::vector<StateSystem> _systems;

	};
}