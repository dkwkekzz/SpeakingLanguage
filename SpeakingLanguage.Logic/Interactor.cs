using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public static class Interactor
    {
        public static unsafe void Execute(ref Service service)
        {
            ref readonly var colAct = ref service.colAct;
            ref readonly var colObj = ref service.colObj;
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

            var requester = service.GetRequester<Interaction>();
            while (requester.TryPop(out Interaction stInter))
            {
                var pSubject = colObj.Find(stInter.lhs);
                if (null == pSubject)
                {
                    Library.Tracer.Error($"no found subject with handle at streaming: {stInter.lhs.ToString()}");
                    continue;
                }

                var pTarget = colObj.Find(stInter.rhs);
                if (null == pTarget)
                {
                    Library.Tracer.Error($"no found target with handle at streaming: {stInter.rhs.ToString()}");
                    continue;
                }

                var dupStateSync = new DupStateSync();

                Library.umnChunk** subjectStateChks = stackalloc Library.umnChunk*[128];
                Library.umnChunk** targetStateChks = stackalloc Library.umnChunk*[128];
                
                var subjectIter = pSubject->GetEnumerator();
                var subjectStateTypeHandle = -1;
                while (subjectIter.MoveNext())
                {
                    var chk = subjectIter.Current;
                    if (subjectStateTypeHandle == chk->typeHandle)
                        continue;

                    subjectStateTypeHandle = chk->typeHandle;
                    subjectStateChks[subjectStateTypeHandle] = chk;

                    dupStateSync.subject.Insert(subjectStateTypeHandle);
                }

                var targetStateIter = pTarget->GetEnumerator();
                var targetStateTypeHandle = -1;
                while (targetStateIter.MoveNext())
                {
                    var chk = targetStateIter.Current;
                    if (targetStateTypeHandle == chk->typeHandle)
                        continue;

                    targetStateTypeHandle = chk->typeHandle;
                    targetStateChks[targetStateTypeHandle] = chk;

                    dupStateSync.target.Insert(subjectStateTypeHandle);
                }

                using (var subject = colObj.GetStateManager(pSubject, subjectStateChks),
                        var target = colObj.GetStateManager(pTarget, targetStateChks))
                var actionCtx = new ActionContext
                {
                    delta = service.frameManager.Delta,
                };

                // 각자는 아직 self를 진행하지 않았다면 진행해야 한다.
                colAct.InvokeSelf(ref actionCtx, ref dupStateSync.subject);
                //colAct.InvokeSelf(ref actionCtx, ref dupStateSync.target);

                colAct.InvokeComplex(ref actionCtx, ref dupStateSync);
            }

            requester.Dispose();
        }
    }
}
