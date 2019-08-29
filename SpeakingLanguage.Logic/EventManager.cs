using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace SpeakingLanguage.Logic
{
    public class EventManager
    {
        private static readonly Lazy<EventManager> lazy = new Lazy<EventManager>(() => new EventManager());
        
        private readonly DataList<Controller> _controllers = new DataList<Controller>(32);
        private readonly DataList<Interaction> _interactions = new DataList<Interaction>(32, new InteractionComparer());

        private readonly List<int> _tempList = new List<int>();
        private readonly Dictionary<int, int> _tempDic = new Dictionary<int, int>();
        private readonly Queue<int> _tempQueue = new Queue<int>();

        private int _currentFrame;
        private int _lastFrame;

        public static EventManager Locator => lazy.Value;
        public static bool IsCreated => lazy.IsValueCreated;

        public int CurrentFrame => _currentFrame;

        public void Insert(int frame, Controller stEvent)
        {
            Library.Tracer.Assert(_currentFrame <= frame);
            Library.Tracer.Assert(_lastFrame <= frame);
            _lastFrame = frame;
            
            _controllers.Add(new DataPair<Controller>
            {
                frame = frame,
                data = stEvent,
            });
        }

        public void Insert(int frame, Interaction stEvent)
        {
            Library.Tracer.Assert(_currentFrame <= frame);
            Library.Tracer.Assert(_lastFrame <= frame);
            _lastFrame = frame;

            var lhsValue = stEvent.lhs.value;
            var rhsValue = stEvent.rhs.value;
            if (lhsValue == rhsValue)
            {
                Library.Tracer.Error($"could not self interact: {lhsValue.ToString()} to {rhsValue.ToString()}");
                return;
            }

            _interactions.Add(new DataPair<Interaction>
            {
                frame = frame,
                data = new Interaction
                {
                    lhs = lhsValue < rhsValue ? stEvent.lhs : stEvent.rhs,
                    rhs = lhsValue < rhsValue ? stEvent.rhs : stEvent.lhs,
                }
            });
        }

        public void ExecuteFrame(ref Service service)
        {
            Interlocked.Increment(ref _currentFrame);

            Process.Director.Execute(this, ref service);
            Process.Interactor.Execute(this, ref service);
            Process.Factory.Execute(this, ref service);

            _commit();
        }

        #region INTERNAL
        internal DataList<Controller>.Enumerator GetControllerEnumerator()
        {
            return _controllers.GetEnumerator();
        }

        internal InteractionGraph GetInteractionGraph()
        {
            _tempList.Clear();
            _tempDic.Clear();
            _tempQueue.Clear();

            var list = _interactions;
            var selectedKey = -1;
            var index = 0;
            var iter = list.GetSortedEnumerator();
            while (iter.MoveNext())
            {
                var key = iter.Current.lhs.value;
                if (selectedKey != key)
                {
                    _tempDic.Add(key, index);
                    _tempList.Add(-1);   // token
                    _tempList.Add(key);  // key

                    selectedKey = key;
                }

                _tempList.Add(iter.Current.rhs.value);
                index++;
            }

            return new InteractionGraph(_tempList, _tempDic, _tempQueue);
        }

        private void _commit()
        {
            _controllers.Swap();
            _interactions.Swap();
        }
        #endregion
    }
}
