#include "stdafx.h"
#include "Core.h"
#include "Process\Notifier.h"

using namespace SpeakingLanguage::Core;

static std::map<int, ProgressCallback> actions;

LogicResult
__cdecl
RegistAction(int key, ProgressCallback progressCallback)
{
	actions.emplace(key, progressCallback);
	return LogicResult();
}

LogicResult
__cdecl
Sample(int val)
{
	std::cout << "start Sample." << std::endl;

	StartInfo info;
	info.default_workercount = 4;
	info.default_jobchunklength = 10;

	auto notifier = new Notifier(info);
	notifier->Awake();

	while (true)
	{
		notifier->Signal();
		std::this_thread::sleep_for(1000ms);
	}
	notifier->Stop();

	std::cout << "stop Sample." << std::endl;

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