#pragma once

#define ERR_MSG(x) x,

enum class Error : unsigned char
{
#include "ErrorDefine.h"
};

#undef ERR_MSG
#define ERR_MSG(x) #x,

static constexpr const char* ErrorMessageNames[] =
{
#include "ErrorDefine.h"
};

#undef ERR_MSG

static inline constexpr const char* GetErrorName(Error err) { return ErrorMessageNames[(int)err]; }