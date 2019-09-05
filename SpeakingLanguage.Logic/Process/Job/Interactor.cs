using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Process
{
    internal static class Interactor
    {
        public static unsafe void Execute(ref Service service, ref Container.InteractGroup group)
        {
            ref readonly var colAct = ref service.colAct;
            ref readonly var colObj = ref service.colObj;
            
            // 일단... 스윕버퍼는 취소
            // 여기서는 실제메모리에 모두 쓴다.
            // 추가되는 상태는 별도 커맨드 버퍼로 넘긴다.
            // 처리방법은 

            var interIter = group.GetEnumerator();
            while (interIter.MoveNext())
            {
                var subjectPair = interIter.Current;
                var selectedSubjectHandle = subjectPair.handle;
                var pSubject = colObj.Find(selectedSubjectHandle);
                if (null == pSubject)
                {
                    Library.Tracer.Error($"[Logic::Process::Interactor] No found subject with handle: {selectedSubjectHandle.ToString()}");
                    continue;
                }

                // set subject frame
                pSubject->frame = service.FrameCount;

                // invoke subject
                var stateSyncPair = new StateSyncPair();

                var subjectStateLookup = stackalloc Library.umnChunk*[8 * sizeof(StateSync)];
                var subjectIter = pSubject->GetEnumerator();
                var subjectStateTypeHandle = -1;
                while (subjectIter.MoveNext())
                {
                    var chk = subjectIter.Current;
                    if (subjectStateTypeHandle == chk->typeHandle)
                        continue;

                    subjectStateTypeHandle = chk->typeHandle;
                    subjectStateLookup[subjectStateTypeHandle] = chk;

                    stateSyncPair.subject.Insert(subjectStateTypeHandle);
                }
                
                var actionCtx = new slActionContext
                {
                    subject = new slObjectContext(pSubject, subjectStateLookup),
                    delta = service.Delta,
                    beginTick = service.BeginTick,
                    currentTick = service.CurrentTick,
                };
                
                colAct.InvokeSelf(ref actionCtx, ref stateSyncPair.subject);

                var subjectLogicState = slObjectHelper.GetDefaultState(pSubject);
                while (interIter.MoveNextChild())
                {
                    var targetPair = interIter.Current;
                    var selectedTargetHandle = targetPair.handle;
                    var pTarget = colObj.Find(selectedTargetHandle);
                    if (null == pTarget)
                    {
                        Library.Tracer.Error($"[Logic::Process::Interactor] No found target with handle: {selectedTargetHandle.ToString()}");
                        continue;
                    }

                    var targetLogicState = slObjectHelper.GetDefaultState(pTarget);
                    if (!StateHelper.ValidateInteract(subjectLogicState, targetLogicState))
                    {
                        Library.Tracer.Error($"[Logic::Process::Interactor] Fail to validate interation: {selectedSubjectHandle.ToString()} to {selectedTargetHandle.ToString()}");
                        continue;
                    }

                    // invoke subject and target
                    stateSyncPair.target.Clear();

                    var targetStateLookup = stackalloc Library.umnChunk*[8 * sizeof(StateSync)];
                    var targetStateIter = pTarget->GetEnumerator();
                    var targetStateTypeHandle = -1;
                    while (targetStateIter.MoveNext())
                    {
                        var chk = targetStateIter.Current;
                        if (targetStateTypeHandle == chk->typeHandle)
                            continue;

                        targetStateTypeHandle = chk->typeHandle;
                        targetStateLookup[targetStateTypeHandle] = chk;

                        stateSyncPair.target.Insert(subjectStateTypeHandle);
                    }
                    
                    actionCtx.target = new slObjectContext(pTarget, targetStateLookup);

                    colAct.InvokeComplex(ref actionCtx, ref stateSyncPair);
                }
            }

        }

        //public static unsafe void Execute2(EventManager eventManager, ref Service service)
        //{
        //    ref readonly var colAct = ref service.colAct;
        //    ref readonly var colObj = ref service.colObj;
        //
        //    // complex interaction
        //
        //    var graph = eventManager.GetInteractionGraph();
        //    var exist = graph.MoveNext();
        //    while (exist)
        //    {
        //        var selectedSubjectHandle = graph.CurrentKey;
        //        var pSubject = colObj.Find(selectedSubjectHandle);
        //        if (null == pSubject)
        //        {
        //            Library.Tracer.Error($"[Logic::Process::Interactor] No found subject with handle: {selectedSubjectHandle.ToString()}");
        //            exist = graph.MoveNext();
        //            continue;
        //        }
        //
        //        // set subject frame
        //        pSubject->frame = eventManager.CurrentFrame;
        //
        //        // invoke subject
        //        var stateSyncPair = new StateSyncPair();
        //
        //        var subjectStateLookup = stackalloc Library.umnChunk*[8 * sizeof(StateSync)];
        //        var subjectIter = pSubject->GetEnumerator();
        //        var subjectStateTypeHandle = -1;
        //        while (subjectIter.MoveNext())
        //        {
        //            var chk = subjectIter.Current;
        //            if (subjectStateTypeHandle == chk->typeHandle)
        //                continue;
        //
        //            subjectStateTypeHandle = chk->typeHandle;
        //            subjectStateLookup[subjectStateTypeHandle] = chk;
        //
        //            stateSyncPair.subject.Insert(subjectStateTypeHandle);
        //        }
        //
        //        byte* subjectStateStack = stackalloc byte[1024];
        //        var actionCtx = new slActionContext
        //        {
        //            subject = new slObjectContext(pSubject, subjectStateLookup, subjectStateStack),
        //            delta = service.Delta,
        //            beginTick = service.BeginTick,
        //            currentTick = service.CurrentTick,
        //        };
        //
        //        colAct.InvokeSelf(ref actionCtx, ref stateSyncPair.subject);
        //
        //        var subjectLogicState = slObjectHelper.GetDefaultState(pSubject);
        //        do
        //        {
        //            var selectedTargetHandle = graph.CurrentValue;
        //            var pTarget = colObj.Find(selectedTargetHandle);
        //            if (null == pTarget)
        //            {
        //                Library.Tracer.Error($"[Logic::Process::Interactor] No found target with handle: {selectedTargetHandle.ToString()}");
        //                exist = graph.MoveNext();
        //                continue;
        //            }
        //
        //            var targetLogicState = slObjectHelper.GetDefaultState(pTarget);
        //            if (!ValidateHelper.ValidateInteract(subjectLogicState, targetLogicState))
        //            {
        //                Library.Tracer.Error($"[Logic::Process::Interactor] Fail to validate interation: {selectedSubjectHandle.ToString()} to {selectedTargetHandle.ToString()}");
        //                exist = graph.MoveNext();
        //                continue;
        //            }
        //
        //            // invoke subject and target
        //            stateSyncPair.target.Clear();
        //
        //            var targetStateLookup = stackalloc Library.umnChunk*[128];
        //            var targetStateIter = pTarget->GetEnumerator();
        //            var targetStateTypeHandle = -1;
        //            while (targetStateIter.MoveNext())
        //            {
        //                var chk = targetStateIter.Current;
        //                if (targetStateTypeHandle == chk->typeHandle)
        //                    continue;
        //
        //                targetStateTypeHandle = chk->typeHandle;
        //                targetStateLookup[targetStateTypeHandle] = chk;
        //
        //                stateSyncPair.target.Insert(subjectStateTypeHandle);
        //            }
        //
        //            byte* targetStateStack = stackalloc byte[1024];
        //            actionCtx.target = new slObjectContext(pTarget, targetStateLookup, targetStateStack);
        //            
        //            colAct.InvokeComplex(ref actionCtx, ref stateSyncPair);
        //
        //            exist = graph.MoveNext();
        //        }
        //        while (selectedSubjectHandle.Equals(graph.CurrentKey));
        //
        //        colObj.InsertBack(ref actionCtx.subject);
        //    }
        //    
        //    // self interaction
        //
        //    var objIter = colObj.GetEnumerator();
        //    while (objIter.MoveNext())
        //    {
        //        var pSelf = objIter.Current;
        //        if (pSelf->frame == eventManager.CurrentFrame)
        //            continue;
        //
        //        pSelf->frame = eventManager.CurrentFrame;
        //
        //        var selfStateSync = new StateSync();
        //
        //        var subjectStateLookup = stackalloc Library.umnChunk*[128];
        //        var subjectIter = pSelf->GetEnumerator();
        //        var subjectStateTypeHandle = -1;
        //        while (subjectIter.MoveNext())
        //        {
        //            var chk = subjectIter.Current;
        //            if (subjectStateTypeHandle == chk->typeHandle)
        //                continue;
        //
        //            subjectStateTypeHandle = chk->typeHandle;
        //            subjectStateLookup[subjectStateTypeHandle] = chk;
        //
        //            selfStateSync.Insert(subjectStateTypeHandle);
        //        }
        //
        //        byte* subjectStateStack = stackalloc byte[1024];
        //        var actionCtx = new slActionContext
        //        {
        //            subject = new slObjectContext(pSelf, subjectStateLookup, subjectStateStack),
        //            delta = service.Delta,
        //            beginTick = service.BeginTick,
        //            currentTick = service.CurrentTick,
        //        };
        //
        //        colAct.InvokeSelf(ref actionCtx, ref selfStateSync);
        //
        //        colObj.InsertBack(ref actionCtx.subject);
        //    }
        //}
    }
}
