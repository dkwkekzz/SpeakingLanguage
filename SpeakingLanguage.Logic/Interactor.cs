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

                Library.umnChunk** stateChks = stackalloc Library.umnChunk*[128];

                var stateIter = pSelf->GetEnumerator();
                var stateTypeHandle = -1;
                while (stateIter.MoveNext())
                {
                    var chk = stateIter.Current;
                    if (stateTypeHandle == chk->typeHandle)
                        continue;

                    stateTypeHandle = chk->typeHandle;
                    stateChks[stateTypeHandle] = chk;
                }

                var actionCtx = new ActionContext
                {
                    subject = new StateContext(pSelf->handle, stateChks),
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

                var dupStateFlag = DupStateSync.Create(pSubject->GetEnumerator(), pTarget->GetEnumerator());

                Library.umnChunk** subjectStateChks = stackalloc Library.umnChunk*[128];
                Library.umnChunk** targetStateChks = stackalloc Library.umnChunk*[128];

                var subjectStateIter = pSubject->GetEnumerator();
                while (subjectStateIter.MoveNext())
                {
                    var chk = subjectStateIter.Current;
                    subjectStateChks[chk->typeHandle] = chk;
                }

                var subjectIter = pSubject->GetEnumerator();
                var subjectStateTypeHandle = -1;
                while (subjectIter.MoveNext())
                {
                    var chk = subjectIter.Current;
                    if (subjectStateTypeHandle == chk->typeHandle)
                        continue;

                    subjectStateTypeHandle = chk->typeHandle;
                    subjectStateChks[subjectStateTypeHandle] = chk;
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
                }

                var actionCtx = new ActionContext
                {
                    subject = new StateContext(stInter.lhs, subjectStateChks),
                    target = new StateContext(stInter.rhs, targetStateChks),
                    delta = service.frameManager.Delta,
                };

                colAct.InvokeComplex(ref actionCtx, ref dupStateFlag);
            }

            requester.Dispose();
        }
    }
}
