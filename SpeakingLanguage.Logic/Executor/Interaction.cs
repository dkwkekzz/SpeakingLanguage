using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal static class Interaction
    {
        public unsafe class InteractContext : IPublicContext
        {
            public float Delta { get; set; }
            public int Frame { get; set; }
            public float FrameTick { get; set; }

            public WorldActor This { get; set; }
            public WorldActor Other { get; set; }
        }

        public static unsafe void Execute(Locator terminal)
        {
            var dicAction = terminal.ActionDictionary;
            var dicOb = terminal.ObserverDictionary;

            var selfRequester = terminal.FrontStack.GetRequester<Event.SelfInteraction>();
            while (!selfRequester.IsEmpty)
            {
                var itr = selfRequester.Pop();
                if (!dicOb.TryGetValue(itr->lhs, out sObserver* lhsOb))
                    return;

                _itrCtx.This = new WorldActor(lhsOb);
                _itrCtx.Other = new WorldActor(null);

                var head = lhsOb->EntityHead;
                while (null != head)
                {
                    var typeHandle = head->typeHandle;
                    if (_actionTable.TryGetValue(new ActionType { typeHandle = typeHandle, relation = Define.Relation.Self }, out IAction action))
                    {
                        action.Invoke(new CallContext { itrCtx = _itrCtx, src = head->ptr.ToPointer() });
                    }

                    head = head->next;
                }
            }

            var simpleRequester = terminal.FrontStack.GetRequester<Event.SimpleInteraction>();
            while (!simpleRequester.IsEmpty)
            {
                var itr = simpleRequester.Pop();
                if (!dicOb.TryGetValue(itr->lhs, out sObserver* lhsOb))
                    return;

                if (!dicOb.TryGetValue(itr->rhs, out sObserver* rhsOb))
                    return;

                _itrCtx.This = new WorldActor(lhsOb);
                _itrCtx.Other = new WorldActor(rhsOb);

                var head = lhsOb->EntityHead;
                while (null != head)
                {
                    var typeHandle = head->typeHandle;
                    if (_actionTable.TryGetValue(new ActionType { typeHandle = typeHandle, relation = Define.Relation.Simple }, out IAction action))
                    {
                        action.Invoke(new CallContext { itrCtx = _itrCtx, src = head->ptr.ToPointer() });
                    }

                    head = head->next;
                }
            }
        }
    }
}
