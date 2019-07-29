using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Interact
{
    internal sealed class Service : IDisposable
    {
        private readonly Library.umnMarshal _marshal;
        private readonly Library.umnHeap _interacHeap;
        private readonly Library.umnFactory<Library.umnHeap, sObserver> _sfOb;
        private readonly Library.umnSplayBT<Library.umnHeap, sObserver> _sbtOb;

        private readonly ActionTable _actionTable;
        private readonly TypeTable _typeTable;
        private readonly Commander _commander;

        //private readonly Dictionary<int, Observer> _dicObs;
        //private readonly Library.ObjectPool<Observer> _poolObs;

        private InteractContext _itrCtx;
        private CallContext _ctx;

        public Service(ref StartInfo info)
        {
            _interacHeap = new Library.umnHeap(info.max_byte_interact_service);
            
            _actionTable = new ActionTable();
            _typeTable = new TypeTable();
            _commander = new Commander(Config.MAX_COUNT_COMMAND_TYPE);
            //_dicObs = new Dictionary<int, Observer>(_capacity);
            //_poolObs = new Library.ObjectPool<Observer>(_capacity);

            _itrCtx = new InteractContext
            {
                This = null,
                TypeTable = _typeTable,
                Commander = _commander,
            };
            _ctx = new CallContext
            {
                delta = 0f,
                src = null,
                args = new object[10]
            };
        }

        public void OnEvent(Terminal terminal)
        {
            _ctx.delta = terminal.Delta;
            _itrCtx.Delta = terminal.Delta;
            _itrCtx.Frame = terminal.Ticker.FrameCount; 
            _itrCtx.FrameTick = terminal.Ticker.CurrentTick;

            _executeExchangeState(terminal);

            _popSelfInteraction(terminal);

            _popSimpleInteraction(terminal);

            _popDestructionInteraction(terminal);

            _popCreationInteraction(terminal);

            _executeCommand(terminal);
        }

        private unsafe void _executeExchangeState(Terminal terminal)
        {
            var requester = terminal.GetBackRequester<Event.Controller>();
            while (!requester.IsEmpty)
            {
                var itr = requester.Pop();
                if (!_dicObs.TryGetValue(itr->handle, out Observer ob))
                    continue;

                if (itr->operation == 0)
                {
                    ob.State.Set(itr->type, itr->value);
                }
            }
        }
            
        private unsafe void _popSelfInteraction(Terminal terminal)
        {
            var requester = terminal.GetRequester<Event.SelfInteraction>();
            while (!requester.IsEmpty)
            {
                var itr = requester.Pop();
                if (!_dicObs.TryGetValue(itr->lhs, out Observer lhsOb))
                    return;

                _itrCtx.This = lhsOb.EntityManager;
                _ctx.args[0] = _itrCtx;

                var iter = lhsOb.EntityManager.GetEnumerator();
                while (iter.MoveNext())
                {
                    var type = iter.Current.Key;
                    var ptr = iter.Current.Value;
                    if (_actionTable.TryGetValue(new ActionType { type = type, relation = Define.Relation.Self }, out IAction action))
                    {
                        _ctx.src = ptr.ToPointer();
                        action.Invoke(ref _ctx);
                    }
                }
            }
        }

        private unsafe void _popSimpleInteraction(Terminal terminal)
        {
            var requester = terminal.GetRequester<Event.SimpleInteraction>();
            while (!requester.IsEmpty)
            {
                var itr = requester.Pop();
                if (!_dicObs.TryGetValue(itr->lhs, out Observer lhsOb))
                    return;

                if (!_dicObs.TryGetValue(itr->rhs, out Observer rhsOb))
                    return;

                _itrCtx.This = lhsOb.EntityManager;
                _ctx.args[0] = _itrCtx;
                _ctx.args[1] = rhsOb.EntityManager;

                var iter = lhsOb.EntityManager.GetEnumerator();
                while (iter.MoveNext())
                {
                    var type = iter.Current.Key;
                    var ptr = iter.Current.Value;
                    if (_dicActions.TryGetValue(new ActionType { type = type, relation = Define.Relation.Simple }, out IAction action))
                    {
                        _ctx.src = ptr.ToPointer();
                        action.Invoke(ref _ctx);
                    }
                }
            }
        }

        private unsafe void _popDestructionInteraction(Terminal terminal)
        {
            var requester = terminal.GetRequester<Event.Destruction>();
            while (!requester.IsEmpty)
            {
                var dst = requester.Pop();
                _destroyObject(dst->type, dst->value);
            }
        }

        private unsafe void _popCreationInteraction(Terminal terminal)
        {
            var requester = terminal.GetRequester<Event.Creation>();
            while (!requester.IsEmpty)
            {
                var crt = requester.Pop();
                _createObject(crt->type, crt->value, 1, 0);
            }
        }

        private unsafe void _executeCommand(Terminal terminal)
        {
            var crtCmd = _commander.GetCommand(typeof(Event.Creation));
            if (!crtCmd.IsEmtpy)
            {
                var crt = new Event.Creation();
                crtCmd.BeginRead();
                while (crtCmd.Read(&crt))
                {
                    _createObject(crt.type, crt.value, 1, 0);
                }
            }

            var dstCmd = _commander.GetCommand(typeof(Event.Destruction));
            if (!dstCmd.IsEmtpy)
            {
                var dst = new Event.Destruction();
                crtCmd.BeginRead();
                while (dstCmd.Read(&dst))
                {
                    _destroyObject(dst.type, dst.value);
                }
            }

            var trCmd = _commander.GetCommand(typeof(Event.Transform));
            if (!trCmd.IsEmtpy)
            {
                var trPoster = terminal.GetPoster<Event.Transform>();
                var ptr = trCmd.GetBuffer(out long sz);
                trPoster.Push(ptr, (int)sz);
            }
        }

        private void _createObject(int type, int value, int count, int delay)
        {
            if (type == (int)Define.Object.Observer)
            {
                if (_poolObs.Capacity == 0)
                    Library.ThrowHelper.ThrowCapacityOverflow("in _poolObs");

                var handle = value;
                var ob = _poolObs.GetObject();
                ob.Take(handle);

                if (_dicObs.Count >= _capacity)
                    Library.ThrowHelper.ThrowCapacityOverflow("in _dicObs");

                _dicObs.Add(handle, ob);

                Database.LoadAsync($"obj_{type.ToString()}_{handle.ToString()}", Define.Object.Observer);
            }
        }

        private void _destroyObject(int type, int value)
        {
            if (type == (int)Define.Object.Observer)
            {
                var handle = value;
                var ob = _dicObs[handle];
                ob.Release();

                _poolObs.PutObject(ob);
            }
        }

        public void Dispose()
        {
            _marshal.Dispose();
        }
    }
}
