// stdafx.h : 자주 사용하지만 자주 변경되지는 않는
// 표준 시스템 포함 파일 또는 프로젝트 특정 포함 파일이 들어 있는
// 포함 파일입니다.
//

#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN             // 거의 사용되지 않는 내용을 Windows 헤더에서 제외합니다.
// Windows 헤더 파일
#include <windows.h>

#ifdef _DEBUG
#define TRACE_STD
#define TRACE_OUTPUT
#endif

// 여기서 프로그램에 필요한 추가 헤더를 참조합니다.
#include <iostream>
#include <vector>
#include <string>
#include <map>
#include <algorithm>
#include <atomic>
#include <queue>
#include <concurrent_queue.h>
#include <thread>
#include <chrono>
#include <exception>
#include <unordered_map>
#include <functional>
#include <cstring>
#include <cassert>
#include <cstdlib>
using namespace std::chrono_literals;

#define assertmsg(a,b) assert(a && b)
