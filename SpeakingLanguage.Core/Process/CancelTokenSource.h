#pragma once
#include "stdafx.h"

namespace SpeakingLanguage { namespace Core
{
	class CancelTokenSource
	{
	public:
		class Token
		{
		private:
			const CancelTokenSource* const _pSrc;
		public:
			explicit Token(CancelTokenSource* src) : _pSrc(src) { }

			inline bool IsCancel() const { return _pSrc == nullptr || _pSrc->GetValue() > 0; }
		};

		explicit CancelTokenSource() {}
		~CancelTokenSource() {}

		inline bool GetValue() const { return _value; }
		inline void SetValue(int val) { _value = val; }
		Token GetToken() { return Token(this); }

	private:
		int _value{0};
	};
} 
}
