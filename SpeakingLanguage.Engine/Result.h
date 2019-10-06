#pragma once
#include <ostream>
#include "Error.h"
#include "tracer.h"

namespace SpeakingLanguage
{
	template<typename T>
	struct Result
	{
		T val;
		const Error error;

		explicit Result(T v) : val(v), error(Error::None) {}
		Result(Error err) : error(err) { tracer::error(GetErrorName(err)); }
		operator T() { return val; }

		inline bool Success() const { return error == Error::None; }
	};

	template<>
	struct Result<void>
	{
		const Error error;

		explicit Result() : error(Error::None) {}
		Result(Error err) : error(err) { tracer::error(GetErrorName(err)); }

		inline bool Success() { return error == Error::None; }
	};

	template<typename T>
	inline Result<T> Done(T res) { return Result<T>(res); }
	inline Result<void> Done() { return Result<void>(); }

	template<typename T>
	std::ostream& operator<<(std::ostream& os, const Result<T>& res)
	{
		os << GetErrorName(res.error) << std::endl;
		return os;
	}
}
