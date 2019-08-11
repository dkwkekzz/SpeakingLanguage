using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Executor
{
    internal static class Interaction
    {
        public static unsafe void Execute(Locator terminal)
        {
            var mngFrame = terminal.FrameManager;
            var dicAction = terminal.ActionDictionary;
            var dicOb = terminal.ObserverDictionary;

            var selfRequester = terminal.FrontStack.GetRequester<Event.SelfInteraction>();
            while (!selfRequester.IsEmpty)
            {
                var itr = selfRequester.Pop();
                if (!dicOb.TryGetValue(itr->lhs, out slObject* lhsOb))
                    return;
                
                var head = lhsOb->EntityHead;
                while (null != head)
                {
                    var typeHandle = head->typeHandle;
                    if (dicAction.TryGetValue(typeHandle, Define.Relation.Self, out IAction action))
                    {
                        var ctx = new InteractContext
                        {
                            delta = mngFrame.Delta,
                            frameTick = mngFrame.FrameTick,
                            pSrc = head->Ptr.ToPointer(),
                            pThis = lhsOb,
                            pOther = null
                        };
                        action.Invoke(&ctx);
                    }

                    head = head->next;
                }
            }

            var simpleRequester = terminal.FrontStack.GetRequester<Event.SimpleInteraction>();
            while (!simpleRequester.IsEmpty)
            {
                var itr = simpleRequester.Pop();
                if (!dicOb.TryGetValue(itr->lhs, out slObject* lhsOb))
                    return;

                if (!dicOb.TryGetValue(itr->rhs, out slObject* rhsOb))
                    return;
                
                var head = lhsOb->EntityHead;
                while (null != head)
                {
                    var typeHandle = head->typeHandle;
                    if (dicAction.TryGetValue(typeHandle, Define.Relation.Simple, out IAction action))
                    {
                        var ctx = new InteractContext
                        {
                            delta = mngFrame.Delta,
                            frameTick = mngFrame.FrameTick,
                            pSrc = head->Ptr.ToPointer(),
                            pThis = lhsOb,
                            pOther = rhsOb
                        };
                        action.Invoke(&ctx);
                    }

                    head = head->next;
                }
            }
        }
    }
}
