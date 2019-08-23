using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public static class Interactor
    {
        public static unsafe void Execute(ref Service service)
        {
            var objIter = colObj.GetEnumerator();
            while (objIter.MoveNext())
            {
                var pSelf = objIter.Current;
                var selfStateSync = StateSync.Create(pSelf->GetEnumerator());

                Library.umnChunk** selfStateLookup = stackalloc Library.umnChunk*[128];

                var stateIter = pSelf->GetEnumerator();
                var stateTypeHandle = -1;
                while (stateIter.MoveNext())
                {
                    var chk = stateIter.Current;
                    if (stateTypeHandle == chk->typeHandle)
                        continue;

                    stateTypeHandle = chk->typeHandle;
                    selfStateLookup[stateTypeHandle] = chk;
                }

                var actionCtx = new ActionContext
                {
                    subject = colObj.GetStateManager(pSelf, selfStateLookup),
                    delta = service.frameManager.Delta,
                };

                colAct.InvokeSelf(ref actionCtx, ref selfStateSync);
            }

            // line

            ref readonly var colAct = ref service.colAct;
            ref readonly var colObj = ref service.colObj;
            var currentFrame = service.frameManager.FrameCount;

            var requester = service.GetRequester<Interaction>();
            
            var exist = requester.TryPop(out Interaction stInter);
            while (exist)
            {
                var selectedSubjectHandle = stInter.lhs;
                var pSubject = colObj.Find(selectedSubjectHandle);
                if (null == pSubject)
                {
                    Library.Tracer.Error($"no found subject with handle at streaming: {selectedSubjectHandle.ToString()}");
                    continue;
                }

                var dupStateSync = new DupStateSync();

                var subjectStateLookup = stackalloc Library.umnChunk*[128];
                var subjectIter = pSubject->GetEnumerator();
                var subjectStateTypeHandle = -1;
                while (subjectIter.MoveNext())
                {
                    var chk = subjectIter.Current;
                    if (subjectStateTypeHandle == chk->typeHandle)
                        continue;

                    subjectStateTypeHandle = chk->typeHandle;
                    subjectStateLookup[subjectStateTypeHandle] = chk;

                    dupStateSync.subject.Insert(subjectStateTypeHandle);
                }

                byte* subjectStateStack = stackalloc byte[1024];
                var actionCtx = new ActionContext
                {
                    subject = new StateManager(pSubject, subjectStateLookup, subjectStateStack),
                    delta = service.frameManager.Delta,
                };

                colAct.InvokeSelf(ref actionCtx, ref dupStateSync.subject);

                do
                {
                    var selectedTargetHandle = stInter.rhs;
                    var pTarget = colObj.Find(selectedTargetHandle);
                    if (null == pTarget)
                    {
                        Library.Tracer.Error($"no found target with handle at streaming: {selectedTargetHandle.ToString()}");
                        continue;
                    }

                    dupStateSync.target.Clear();

                    var targetStateLookup = stackalloc Library.umnChunk*[128];
                    var targetStateIter = pTarget->GetEnumerator();
                    var targetStateTypeHandle = -1;
                    while (targetStateIter.MoveNext())
                    {
                        var chk = targetStateIter.Current;
                        if (targetStateTypeHandle == chk->typeHandle)
                            continue;

                        targetStateTypeHandle = chk->typeHandle;
                        targetStateLookup[targetStateTypeHandle] = chk;

                        dupStateSync.target.Insert(subjectStateTypeHandle);
                    }

                    byte* targetStateStack = stackalloc byte[1024];
                    actionCtx.target = new StateManager(pTarget, targetStateLookup, targetStateStack);
                    
                    colAct.InvokeComplex(ref actionCtx, ref dupStateSync);

                    exist = requester.TryPop(out stInter);
                }
                while (selectedSubjectHandle.Equals(stInter.lhs));

                colObj.InsertBack(ref actionCtx.subject);
            }
            
            requester.Dispose();
        }
    }
}
