using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public class CleanManager<TKey>
    {
        class Priority
        {
            public TKey key;
            public int createdTick;
            public int recentlyTick;
            public int reference;
            public Priority next;
            public Priority prev;
        }

        private readonly int _aliveDuration;
        private readonly int _allowedCapacity;
        private readonly ObjectPool<Priority> _pool;
        private readonly Dictionary<TKey, Priority> _root;

        private Action<TKey> _deletor = null;
        private Priority head = null;
        private Priority tail = null;

        public CleanManager(int capacity, int duration, Action<TKey> deletor)
        {
            _allowedCapacity = capacity >> 1;
            _aliveDuration = duration;
            _pool = new ObjectPool<Priority>(capacity);
            _root = new Dictionary<TKey, Priority>(capacity);
            _deletor = deletor;
        }

        public void Link(Action<TKey> deletor)
        {
            _deletor = deletor;
        }

        public void RegistPriority(TKey key)
        {
            var currentTick = Environment.TickCount;
            var prt = _pool.GetObject();
            prt.key = key;
            prt.createdTick = currentTick;
            prt.recentlyTick = currentTick;
            prt.reference = 1;
            prt.next = null;
            prt.prev = tail;
            tail = prt;

            _root.Add(key, prt);
        }

        public void UpdatePriority(TKey key)
        {
            var prt = _root[key];
            prt.recentlyTick = Environment.TickCount;
            prt.reference++;

            _moveToTail(prt);
        }

        public void Clear(int leg)
        {
            if ((_root.Count >= _allowedCapacity && leg >= 5) /*메모리가 적당히 찼으면서 시간이 여유로운 경우*/
                || (_root.Count >= _allowedCapacity << 1) /*메모리가 꽉찬 경우*/)
            {
                _collectGarbage();
            }
        }

        private void _collectGarbage()
        {
            var garbageHead = head;
            var currentTick = Environment.TickCount;
            while (null != garbageHead)
            {
                if (garbageHead.recentlyTick + _aliveDuration > currentTick)
                    break;

                _remove(garbageHead);
                garbageHead = head;
            }
        }
        
        private void _remove(Priority prt)
        {
            var rhs = prt.next;
            var lhs = prt.prev;
            if (null != rhs) rhs.prev = lhs;
            if (null != lhs) lhs.next = rhs;
            if (null == lhs) head = lhs;
            prt.next = null;
            prt.prev = null;

            _root.Remove(prt.key);
            _pool.PutObject(prt);
            _deletor?.Invoke(prt.key);
        }

        private void _remove(TKey key)
        {
            Priority prt;
            if (!_root.TryGetValue(key, out prt))
                return;

            _remove(prt);
        }

        private void _moveToTail(Priority prt)
        {
            var rhs = prt.next;
            var lhs = prt.prev;
            if (null != rhs) rhs.prev = lhs;
            if (null != lhs) lhs.next = rhs;
            if (null == lhs) head = lhs;
            prt.next = null;
            prt.prev = tail;
            tail = prt;
        }

        private void _forwarding(Priority prt)
        {
            var rhs = prt.next;
            var lhs = prt.prev;
            if (null == rhs)
                return;
            if (null == lhs)
                head = rhs;

            prt.prev = rhs;
            prt.next = rhs?.next;

            if (null != lhs)
            {
                lhs.next = rhs;
            }

            if (null != rhs)
            {
                rhs.prev = lhs;
                rhs.next = prt;
            }
        }
    }
}
