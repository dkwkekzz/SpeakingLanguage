#include "stdafx.h"
#include "Interactor.h"
#include "slChunk.h"
#include "slObjectCollection.h"

using namespace SpeakingLanguage::Core;

constexpr int STACK_BUFFER_SIZE = 1024;

Interactor::Interactor()
{
}


Interactor::~Interactor()
{
}

Result<void> 
Interactor::Execute(Service& service, const InteractionGroup& group)
{
	slChunk* subjectStateLookup[8 * sizeof(StateSync)];
	slChunk* targetStateLookup[8 * sizeof(StateSync)];
	BYTE* targetStateLookup[STACK_BUFFER_SIZE];

	for (const auto& interaction : group)
	{

	}

}
//public static unsafe void Execute(ref Service service, ref Data.InteractGroup group)
//{
//	ref readonly var colAct = ref service.colAct;
//	ref var colObj = ref service.colObj;
//
//	var subjectStateLookup = stackalloc Library.umnChunk*[8 * sizeof(StateSync)];
//	var targetStateLookup = stackalloc Library.umnChunk*[8 * sizeof(StateSync)];
//	var tempStackPtr = stackalloc byte[slSubject.STACK_BUFFER_SIZE];
//
//	var interIter = group.GetEnumerator();
//	while (interIter.MoveNext())
//	{
//		var subjectPair = interIter.Current;
//		var selectedSubjectHandle = subjectPair.handle;
//		var pSubject = colObj.Find(selectedSubjectHandle);
//		if (null == pSubject)
//		{
//			Library.Tracer.Error($"[Logic::Process::Interactor] No found subject with handle: {selectedSubjectHandle.ToString()}");
//			continue;
//		}
//
//		// execute default logic
//		var subjectLogicState = slObjectHelper.GetDefaultState(pSubject);
//		var life = subjectLogicState->lifeCycle;
//		if (life.live && life.value == 0)
//		{
//			colObj.Destroy(pSubject);
//			continue;
//		}
//
//		var spawner = subjectLogicState->spawner;
//		if (spawner.value > 0)
//		{
//			colObj.CreateBack(spawner.value);
//		}
//
//		// invoke subject
//		var stateSyncPair = new StateSyncPair();
//		var subjectIter = pSubject->GetEnumerator();
//		var subjectStateTypeHandle = -1;
//		while (subjectIter.MoveNext())
//		{
//			var chk = subjectIter.Current;
//			if (subjectStateTypeHandle == chk->typeHandle)
//				continue;
//
//			subjectStateTypeHandle = chk->typeHandle;
//			subjectStateLookup[subjectStateTypeHandle] = chk;
//
//			stateSyncPair.subject.Insert(subjectStateTypeHandle);
//		}
//
//		var actionCtx = new slActionContext
//		{
//			subject = new slSubject(pSubject, subjectStateLookup, tempStackPtr),
//			delta = service.Delta,
//			beginTick = service.BeginTick,
//			currentTick = service.CurrentTick,
//		};
//
//		colAct.InvokeSelf(ref actionCtx, ref stateSyncPair.subject);
//
//		// iterate targets
//		while (interIter.MoveNextChild())
//		{
//			var targetPair = interIter.Current;
//			var selectedTargetHandle = targetPair.handle;
//			var pTarget = colObj.Find(selectedTargetHandle);
//			if (null == pTarget)
//			{
//				Library.Tracer.Error($"[Logic::Process::Interactor] No found target with handle: {selectedTargetHandle.ToString()}");
//				continue;
//			}
//
//			var targetLogicState = slObjectHelper.GetDefaultState(pTarget);
//			if (!StateHelper.ValidateInteract(subjectLogicState, targetLogicState))
//			{
//				Library.Tracer.Error($"[Logic::Process::Interactor] Fail to validate interation: {selectedSubjectHandle.ToString()} to {selectedTargetHandle.ToString()}");
//				continue;
//			}
//
//			// invoke subject and target
//			stateSyncPair.target.Clear();
//			var targetStateIter = pTarget->GetEnumerator();
//			var targetStateTypeHandle = -1;
//			while (targetStateIter.MoveNext())
//			{
//				var chk = targetStateIter.Current;
//				if (targetStateTypeHandle == chk->typeHandle)
//					continue;
//
//				targetStateTypeHandle = chk->typeHandle;
//				targetStateLookup[targetStateTypeHandle] = chk;
//
//				stateSyncPair.target.Insert(subjectStateTypeHandle);
//			}
//
//			actionCtx.target = new slTarget(pTarget, targetStateLookup);
//
//			colAct.InvokeComplex(ref actionCtx, ref stateSyncPair);
//		}
//
//		colObj.InsertBack(ref actionCtx.subject);
//	}
//
//}