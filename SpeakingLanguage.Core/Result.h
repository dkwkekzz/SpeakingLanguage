#pragma once

namespace SpeakingLanguage {
	namespace Core {

		enum class Error : unsigned char
		{
			None = 0,
			Complete,
			NullReferenceHandle,
			OverflowHandle,
			OutOfMemory,
		};

		template<typename T>
		struct Result
		{
			T val;
			Error error;

			Result(T v) : val(v), error(Error::None) {}
			Result(Error err) : error(err) {}
			operator T() { return val; }

			inline bool Success() const { return error == Error::None; }
		};

		template<>
		struct Result<void>
		{
			const Error error;

			Result() : error(Error::None) {}
			Result(Error err) : error(err) {}

			inline bool Success() { return error == Error::None; }
		};

		template<typename T>
		inline Result<T> Done(T res) { return Result<T>(res); }
		inline Result<void> Done() { return Result<void>(); }

	}
}