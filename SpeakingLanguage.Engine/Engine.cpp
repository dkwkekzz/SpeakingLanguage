#include "stdafx.h"
#include "Engine.h"

static std::map<int, ProgressCallback> actions;

LogicResult
__cdecl
Sample(int val)
{
	std::cout << "Sample" << std::endl;
	return LogicResult();
}

LogicResult
__cdecl
Sample2()
{
	std::cout << "Sample2" << std::endl;
	return LogicResult();
}

LogicResult
__cdecl
RegistAction(int key, ProgressCallback progressCallback)
{
	actions.emplace(key, progressCallback);
	return LogicResult();
}

LogicResult
__cdecl
CreateObject(slObjectHandle* handle)
{
	std::cout << "CreateObject" << std::endl;
	handle->value = 0;
	return LogicResult();
}

LogicResult
__cdecl
DestroyObject(slObjectHandle handle)
{
	std::cout << "DestroyObject: " << handle.value << std::endl;
	return LogicResult();
}

LogicResult
__cdecl
DeserializeObject(BYTE* buffer, slObjectHandle* outHandle)
{
	std::cout << buffer << std::endl;
	return LogicResult();
}

LogicResult
__cdecl
SerializeObject(slObjectHandle handle, BYTE* outBuffer)
{
	auto str = "SerializeObject";
	for (int i = 0; str[i] != '\0'; i++)
		*(outBuffer + i) = str[i];
	return LogicResult();
}

LogicResult
__cdecl
InsertKeyboard(slObjectHandle, int, int)
{
	std::cout << "InsertKeyboard" << std::endl;
	return LogicResult();
}

LogicResult
__cdecl
InsertTouch(slObjectHandle, int, int)
{
	std::cout << "InsertTouch" << std::endl;
	return LogicResult();
}

LogicResult
__cdecl
InsertInteraction(slObjectHandle, slObjectHandle)
{
	std::cout << "InsertInteraction" << std::endl;
	return LogicResult();
}

void
__cdecl
EnterFrame()
{
	std::cout << "EnterFrame" << std::endl;
}

FrameResult
__cdecl
ExecuteFrame()
{
	std::cout << "ExecuteFrame" << std::endl;
	return FrameResult();
}