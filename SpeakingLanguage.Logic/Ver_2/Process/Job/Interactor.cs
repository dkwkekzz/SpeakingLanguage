﻿using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Process
{
    internal static class Interactor
    {
        public static unsafe void Execute(ref Service service, ref Data.InteractGroup group)
        {
            ref readonly var colAct = ref service.colAct;
            ref var colObj = ref service.colObj;

            var subjectStateLookup = stackalloc Library.umnChunk*[8 * sizeof(StateSync)];
            var targetStateLookup = stackalloc Library.umnChunk*[8 * sizeof(StateSync)];
            var tempStackPtr = stackalloc byte[slSubject.STACK_BUFFER_SIZE];

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

                // execute default logic
                var subjectLogicState = slObjectHelper.GetDefaultState(pSubject);
                var life = subjectLogicState->lifeCycle;
                if (life.live && life.value == 0)
                {
                    colObj.Destroy(pSubject);
                    continue;
                }

                var spawner = subjectLogicState->spawner;
                if (spawner.value > 0)
                {
                    colObj.CreateBack(spawner.value);
                }

                // invoke subject
                var stateSyncPair = new StateSyncPair();
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
                    subject = new slSubject(pSubject, subjectStateLookup, tempStackPtr),
                    delta = service.Delta,
                    beginTick = service.BeginTick,
                    currentTick = service.CurrentTick,
                };
                
                colAct.InvokeSelf(ref actionCtx, ref stateSyncPair.subject);

                // iterate targets
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
                    
                    actionCtx.target = new slTarget(pTarget, targetStateLookup);

                    colAct.InvokeComplex(ref actionCtx, ref stateSyncPair);
                }

                colObj.InsertBack(ref actionCtx.subject);
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
