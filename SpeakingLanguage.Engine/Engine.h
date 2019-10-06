#pragma once

typedef void(__stdcall * ProgressCallback)(int);

struct slObjectHandle
{
	int value;
};

enum class LogicError : int
{
	None = 0,
	FailToReadLength,
	FailToReadHandle,
	NullReferenceObject,
	NullReferenceControlState,
	SelfInteraction,
	OverflowObjectCapacity,
};

struct LogicResult
{
	LogicError error;
};

struct FrameResult
{
	int frameCount;
};

extern "C" __declspec(dllexport) LogicResult __cdecl Sample(int);
extern "C" __declspec(dllexport) LogicResult __cdecl Sample2();
extern "C" __declspec(dllexport) LogicResult __cdecl RegistAction(int, ProgressCallback);
extern "C" __declspec(dllexport) LogicResult __cdecl CreateObject(slObjectHandle*);
extern "C" __declspec(dllexport) LogicResult __cdecl DestroyObject(slObjectHandle);
extern "C" __declspec(dllexport) LogicResult __cdecl DeserializeObject(BYTE*, slObjectHandle*);
extern "C" __declspec(dllexport) LogicResult __cdecl SerializeObject(slObjectHandle, BYTE*);
extern "C" __declspec(dllexport) LogicResult __cdecl InsertKeyboard(slObjectHandle, int, int);
extern "C" __declspec(dllexport) LogicResult __cdecl InsertTouch(slObjectHandle, int, int);
extern "C" __declspec(dllexport) LogicResult __cdecl InsertInteraction(slObjectHandle, slObjectHandle);

extern "C" __declspec(dllexport) void		 __cdecl EnterFrame();
extern "C" __declspec(dllexport) FrameResult __cdecl ExecuteFrame();

//public unsafe EventResult<slObjectHandle> CreateObject()
//{
//	var pObj = _logicService.colObj.CreateFront(0);
//	if (null == pObj)
//		return new EventResult<slObjectHandle>(EventError.OverflowObjectCapacity);
//
//	return new EventResult<slObjectHandle>(EventError.None, pObj->handle);
//}
//
//public unsafe EventResult DestroyObject(slObjectHandle handle)
//{
//	var obj = _logicService.colObj.Find(handle);
//	if (obj == null)
//		return new EventResult(EventError.NullReferenceObject);
//
//	_logicService.colObj.Destroy(obj);
//	return new EventResult();
//}
//
//public unsafe EventResult<slObjectHandle> DeserializeObject(ref Library.Reader reader)
//{
//	var read = reader.ReadInt(out int handleValue);
//	if (!read) return new EventResult<slObjectHandle>(EventError.FailToReadHandle);
//
//	if (handleValue == 0)
//	{
//		var pObj = _logicService.colObj.CreateFront(handleValue);
//		if (null == pObj)
//			return new EventResult<slObjectHandle>(EventError.OverflowObjectCapacity);
//	}
//	else
//	{
//		read = reader.ReadInt(out int length);
//		if (!read) return new EventResult<slObjectHandle>(EventError.FailToReadLength);
//
//		var pObj = _logicService.colObj.InsertFront(reader.Buffer, reader.Position, length);
//		if (null == pObj)
//			return new EventResult<slObjectHandle>(EventError.OverflowObjectCapacity);
//	}
//	return new EventResult<slObjectHandle>(EventError.None, handleValue);
//}
//
//public unsafe EventResult SerializeObject(slObjectHandle handle, ref Library.Writer writer)
//{
//	var obj = _logicService.colObj.Find(handle);
//	if (obj == null)
//		return new EventResult(EventError.NullReferenceObject);
//
//	var size = obj->Capacity + sizeof(slObject);
//	writer.WriteInt(handle.value);
//	writer.WriteInt(size);
//	writer.WriteMemory(obj, size);
//
//	return new EventResult();
//}
//
//public unsafe EventResult InsertKeyboard(int subjectHandleValue, int key, int value)
//{
//	var pSubject = _logicService.colObj.Find(subjectHandleValue);
//	if (null == pSubject)
//		return new EventResult(EventError.NullReferenceObject);
//
//	var controlState = slObjectHelper.GetControlState(pSubject);
//	if (null == controlState)
//		return new EventResult(EventError.NullReferenceControlState);
//
//	var consoleKey = (ConsoleKey)key;
//	switch (consoleKey)
//	{
//	case ConsoleKey.LeftArrow:
//		controlState->direction &= value;
//		break;
//	case ConsoleKey.RightArrow:
//		controlState->direction &= (value << 2);
//		break;
//	case ConsoleKey.UpArrow:
//		controlState->direction &= (value << 4);
//		break;
//	case ConsoleKey.DownArrow:
//		controlState->direction &= (value << 8);
//		break;
//	case 0: // ctrl
//		controlState->keyFire &= (value);
//		break;
//	case (ConsoleKey)1: // alt
//		controlState->keyFire &= (value << 1);
//		break;
//	case (ConsoleKey)2: // shift
//		controlState->keyFire &= (value << 2);
//		break;
//	case ConsoleKey.A:
//		controlState->keyFire &= (value << 3);
//		break;
//	case ConsoleKey.S:
//		controlState->keyFire &= (value << 4);
//		break;
//	case ConsoleKey.D:
//		controlState->keyFire &= (value << 5);
//		break;
//	case ConsoleKey.W:
//		controlState->keyFire &= (value << 6);
//		break;
//	}
//	return new EventResult();
//}
//
//public unsafe EventResult InsertTouch(int subjectHandleValue, int target, int fire)
//{
//	var pSubject = _logicService.colObj.Find(subjectHandleValue);
//	if (null == pSubject)
//		return new EventResult(EventError.NullReferenceObject);
//
//	var controlState = slObjectHelper.GetControlState(pSubject);
//	if (null == controlState)
//		return new EventResult(EventError.NullReferenceControlState);
//
//	controlState->touchTarget = target;
//	controlState->touchFire = fire;
//
//	return new EventResult();
//}
//
//public unsafe EventResult InsertInteraction(int subjectHandleValue, int targetHandleValue)
//{
//	if (subjectHandleValue == targetHandleValue)
//		return new EventResult(EventError.SelfInteraction);
//
//	var stInter = new Interaction
//	{
//		subject = subjectHandleValue,
//		target = targetHandleValue,
//	};
//
//	ref readonly var itrGraph = ref _logicService.itrGraph;
//	itrGraph.Insert(ref stInter);
//
//	return new EventResult();
//}
//
//public void FrameEnter()
//{
//	_logicService.Begin();
//	CurrentFrame++;
//}
//
//public FrameResult ExecuteFrame()
//{
//	_notifier.Signal(ref _logicService);
//	return _logicService.End(CurrentFrame);
//}