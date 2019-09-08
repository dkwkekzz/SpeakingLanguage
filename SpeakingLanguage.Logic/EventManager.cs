﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public sealed class EventManager : Library.SingletonLazy<EventManager>
    {
        private Service _logicService;
        private IProcessor _notifier;

        internal ref Service Service => ref _logicService;
        internal int CurrentFrame { get; private set; }
        
        public void Install(ref StartInfo info)
        {
            _logicService = new Service(ref info);
            _notifier = new Process.Notifier(ref info);
            _notifier.Awake();

            CurrentFrame = 0;
        }

        public void Uninstall()
        {
            _logicService.Dispose();
            _notifier.Dispose();
        }

        public unsafe EventResult<int> CreateObject()
        {
            var pObj = _logicService.colObj.CreateFront(0);
            if (null == pObj)
                return new EventResult<int>(EventError.OverflowObjectCapacity);

            return new EventResult<int>(EventError.None, pObj->handle.value);
        }

        public unsafe EventResult<int> DeserializeObject(ref Library.Reader reader)
        {
            var read = reader.ReadInt(out int handleValue);
            if (!read) return new EventResult<int>(EventError.FailToReadHandle);

            if (handleValue == 0)
            {
                var pObj = _logicService.colObj.CreateFront(handleValue);
                if (null == pObj)
                    return new EventResult<int>(EventError.OverflowObjectCapacity);
            }
            else
            {
                read = reader.ReadInt(out int length);
                if (!read) return new EventResult<int>(EventError.FailToReadLength);

                var pObj = _logicService.colObj.InsertFront(reader.Buffer, reader.Position, length);
                if (null == pObj)
                    return new EventResult<int>(EventError.OverflowObjectCapacity);
            }
            return new EventResult<int>(EventError.None, handleValue);
        }

        public unsafe EventResult SerializeObject(slObjectHandle handle, ref Library.Writer writer)
        {
            var obj = _logicService.colObj.Find(handle);
            if (obj == null)
                return new EventResult(EventError.NullReferenceObject);

            var size = obj->Capacity + sizeof(slObject);
            writer.WriteInt(handle.value);
            writer.WriteInt(size);
            writer.WriteMemory(obj, size);

            return new EventResult();
        }

        public unsafe EventResult InsertKeyboard(int subjectHandleValue, int key, int value)
        {
            var pSubject = _logicService.colObj.Find(subjectHandleValue);
            if (null == pSubject)
                return new EventResult(EventError.NullReferenceObject);

            var controlState = slObjectHelper.GetControlState(pSubject);
            if (null == controlState)
                return new EventResult(EventError.NullReferenceControlState);

            var consoleKey = (ConsoleKey)key;
            switch (consoleKey)
            {
                case ConsoleKey.LeftArrow:
                    controlState->direction &= value;
                    break;
                case ConsoleKey.RightArrow:
                    controlState->direction &= (value << 2);
                    break;
                case ConsoleKey.UpArrow:
                    controlState->direction &= (value << 4);
                    break;
                case ConsoleKey.DownArrow:
                    controlState->direction &= (value << 8);
                    break;
                case 0: // ctrl
                    controlState->keyFire &= (value);
                    break;
                case (ConsoleKey)1: // alt
                    controlState->keyFire &= (value << 1);
                    break;
                case (ConsoleKey)2: // shift
                    controlState->keyFire &= (value << 2);
                    break;
                case ConsoleKey.A:
                    controlState->keyFire &= (value << 3);
                    break;
                case ConsoleKey.S:
                    controlState->keyFire &= (value << 4);
                    break;
                case ConsoleKey.D:
                    controlState->keyFire &= (value << 5);
                    break;
                case ConsoleKey.W:
                    controlState->keyFire &= (value << 6);
                    break;
            }
            return new EventResult();
        }

        public unsafe EventResult InsertTouch(int subjectHandleValue, int target, int fire)
        {
            var pSubject = _logicService.colObj.Find(subjectHandleValue);
            if (null == pSubject)
                return new EventResult(EventError.NullReferenceObject);

            var controlState = slObjectHelper.GetControlState(pSubject);
            if (null == controlState)
                return new EventResult(EventError.NullReferenceControlState);

            controlState->touchTarget = target;
            controlState->touchFire = fire;

            return new EventResult();
        }

        public unsafe EventResult InsertInteraction(int subjectHandleValue, int targetHandleValue)
        {
            if (subjectHandleValue == targetHandleValue)
                return new EventResult( EventError.SelfInteraction);

            var stInter = new Interaction
            {
                subject = subjectHandleValue,
                target = targetHandleValue,
            };

            ref readonly var itrGraph = ref _logicService.itrGraph;
            itrGraph.Insert(ref stInter);

            return new EventResult();
        }
        
        public void FrameEnter()
        {
            _logicService.Begin();
            CurrentFrame++;
        }
        
        public FrameResult ExecuteFrame()
        {
            _notifier.Signal(ref _logicService);
            return _logicService.End(CurrentFrame);
        }

        #region INTERNAL
        //internal DataList<Controller>.Enumerator GetControllerEnumerator()
        //{
        //    return _controllers.GetEnumerator();
        //}
        //
        //internal DataList<Interaction>.Enumerator GetInteractionEnumerator()
        //{
        //    return _interactions.GetSortedEnumerator();
        //}
        //
        //internal InteractionGraph GetInteractionGraph()
        //{
        //    _tempList.Clear();
        //    _tempDic.Clear();
        //    _tempQueue.Clear();
        //
        //    var list = _interactions;
        //    var selectedKey = -1;
        //    var index = 0;
        //    var iter = list.GetSortedEnumerator();
        //    while (iter.MoveNext())
        //    {
        //        var key = iter.Current.lhs.value;
        //        if (selectedKey != key)
        //        {
        //            _tempDic.Add(key, index);
        //            _tempList.Add(-1);   // token
        //            _tempList.Add(key);  // key
        //
        //            selectedKey = key;
        //        }
        //
        //        _tempList.Add(iter.Current.rhs.value);
        //        index++;
        //    }
        //
        //    return new InteractionGraph(_tempList, _tempDic, _tempQueue);
        //}
        #endregion
    }
}