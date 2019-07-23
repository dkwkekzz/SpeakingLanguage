using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public sealed class SplayBT<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
        where TKey : IComparable<TKey>
    {
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator
        {
            private SplayBT<TKey, TValue> splay;
            private Node current;
            
            public Enumerator(SplayBT<TKey, TValue> splay)
            {
                this.splay = splay;
                this.current = splay._root;
            }

            public KeyValuePair<TKey, TValue> Current { get { return new KeyValuePair<TKey, TValue> (current.key, current.value); } }
            object IEnumerator.Current { get { return this.Current; } }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                throw new NotImplementedException();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

        }

        class Node
        {
            public Node l, r, p;
            public TKey key;
            public TValue value;
            public int count;
        }

        interface IFactory
        {
            int Capacity { get; }
            Node GetObject();
            void PutObject(Node x);
        }

        class Heap : IFactory
        {
            private ConcurrentBag<Node> _pool = new ConcurrentBag<Node>();
            public int Capacity { get { return _pool.Count; } }

            public Heap(int sz)
            {
                for (int i = 0; i != sz; i++)
                    _pool.Add(new Node());
            }

            public Node GetObject()
            {
                Node item;
                if (_pool.TryTake(out item))
                    return item;

                return new Node();
            }

            public void PutObject(Node item)
            {
                _pool.Add(item);
            }
        }

        private IFactory _factory;
        private IComparer<TKey> _comparer;
        private Node _root;

        public int Capacity { get { return _factory == null ? _root.count : _factory.Capacity; } }
        public int Count { get { return _root == null ? 0 : _root.count; } }
        public TValue this[int k] { get { return Find_Kth(k); } }

        public SplayBT()
        {
        }

        public SplayBT(IComparer<TKey> comparer)
        {
            _comparer = comparer;
        }

        public SplayBT(int capacity)
        {
            _factory = new Heap(capacity);
        }

        public SplayBT(int capacity, IComparer<TKey> comparer)
        {
            _factory = new Heap(capacity);
            _comparer = comparer;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Clear()
        {

        }

        public void Insert(TKey key, TValue value)
        {
            insert(ref key, ref value);
        }

        public bool Delete(TKey key)
        {
            return delete(ref key);
        }

        public bool Find(TKey key, out TValue value)
        {
            return find(ref key, out value);
        }

        public TValue Find_Kth(int k)
        {
            if (_root.count < k)
                throw new OverflowException($"[Find_Kth] index overflow - root.count/k:{_root.count.ToString()}/{k.ToString()}");
            
            var x = find_Kth(k);
            if (null == x)
                return default(TValue);

            return x.value;
        }

        public TValue FirstOrDefault()
        {
            var x = first();
            if (null == x)
                return default(TValue);

            return x.value;
        }

        public TValue LastOrDefault()
        {
            var x = last();
            if (null == x)
                return default(TValue);

            return x.value;
        }

        #region PRIVATE
        private Node next(Node p)
        {
            if (null == _root)
                return null;

            if (d.right)
            {
                successor = d.right;
                while (successor.left) successor = successor.left;
                return successor;
            }

            const comparator = this._comparator;
            while (root)
            {
                const cmp = comparator(d.key, root.key);
                if (cmp === 0) break;
                else if (cmp < 0)
                {
                    successor = root;
                    root = root.left;
                }
                else root = root.right;
            }

            Node x = _root;
            if (null != x.r)
            {
                x = x.r;
                while (null != x.l)
                    x = x.l;
            }
            else
            {
                while (null != x)
                {
                    int ret;
                    if (null != _comparer)
                        ret = _comparer.Compare(p.key, x.key);
                    else
                        ret = x.key.CompareTo(p.key);
                    if (ret == 0)
                        break;
                    if (ret == 1)
                    {
                        if (null == p.l)
                            break;
                        p = p.l;
                    }
                    else
                    {
                        if (null == p.r)
                            break;
                        p = p.r;
                    }
                }
            }

            else if (null != x.p)
            {
                x = x.p;
                if (null == x.r)
                    return null;
                x = x.r;
            }
            else
                return null;

            while (true)
            {
                if (null != x.l)
                    x = x.l;
                else
                    break;
            }

            splay(x);

            return _root;
        }

        private Node first()
        {
            if (null == _root)
                return null;
            
            Node x = _root;
            while (null != x.l)
                x = x.l;

            splay(x);
            
            return _root;
        }

        private Node last()
        {
            if (null == _root)
                return null;

            Node x = _root;
            while (null != x.r)
                x = x.r;

            splay(x);

            return _root;
        }

        private Node find_Kth(int k)
        {
            Node x = _root;
            while (true)
            {
                while (null != x.l && x.l.count > k)
                    x = x.l;
                if (null != x.l)
                    k -= x.l.count;
                if (k-- == 0)
                    break;
                x = x.r;
            }

            splay(x);

            return _root;
        }

        private void rotate(Node x)
        {
            Node p = x.p;
            Node b;
            if (x == p.l)
            {
                p.l = b = x.r;
                x.r = p;
            }
            else
            {
                p.r = b = x.l;
                x.l = p;
            }

            x.p = p.p;
            p.p = x;
            if (null != b)
                b.p = p;

            Node g = x.p;
            if (null != g)
            {
                if (p == g.l)
                    g.l = x;
                else
                    g.r = x;
            }
            else
            {
                _root = x;
            }

            update(p);
            update(x);
        }

        private void splay(Node x)
        {
            while (null != x.p)
            {
                Node p = x.p;
                Node g = p.p;
                if (null != g)
                {
                    bool zigzig = (p.l == x) == (g.l == p);
                    if (zigzig)
                        rotate(p);
                    else
                        rotate(x);
                }

                rotate(x);
            }
        }

        private void insert(ref TKey key, ref TValue value)
        {
            Node p = _root;
            if (null == p)
            {
                _root = createNode(ref key, ref value);
                return;
            }

            bool left;
            while (true)
            {
                int ret;
                if (null != _comparer)
                    ret = _comparer.Compare(key, p.key);
                else
                    ret = p.key.CompareTo(key);
                if (0 == ret)
                    return;
                else if(1 == ret)
                {
                    if (null == p.l)
                    {
                        left = true;
                        break;
                    }
                    p = p.l;
                }
                else
                {
                    if (null == p.r)
                    {
                        left = false;
                        break;
                    }
                    p = p.r;
                }
            }

            var x = createNode(ref key, ref value);
            if (left)
                p.l = x;
            else
                p.r = x;
            x.p = p;
            splay(x);
        }

        private bool find(ref TKey key, out TValue value)
        {
            Node p = _root;
            if (null == p)
            {
                value = default(TValue);
                return false;
            }

            while (null != p)
            {
                int ret;
                if (null != _comparer)
                    ret = _comparer.Compare(key, p.key);
                else
                    ret = p.key.CompareTo(key);
                if (ret == 0)
                    break;
                if (ret == 1)
                {
                    if (null == p.l)
                        break;
                    p = p.l;
                }
                else
                {
                    if (null == p.r)
                        break;
                    p = p.r;
                }
            }

            splay(p);

            bool equal;
            if (null != _comparer)
                equal = _comparer.Compare(key, p.key) == 0;
            else
                equal = p.key.CompareTo(key) == 0;
            if (!equal)
            {
                value = default(TValue);
                return false;
            }

            value = p.value;
            return true;
        }

        private bool delete(ref TKey key)
        {
            TValue value;
            if (!find(ref key, out value))
                return false;

            Node p = _root;
            if (null != p.l)
            {
                if (null != p.r)
                {
                    _root = p.l;
                    _root.p = null;
                    Node x = _root;
                    while (null != x.r)
                        x = x.r;
                    x.r = p.r;
                    p.r.p = x;
                    splay(x);
                    destroyNode(p);
                    return true;
                }
                _root = p.l;
                _root.p = null;
                destroyNode(p);
                return true;
            }

            if (null != p.r)
            {
                _root = p.r;
                _root.p = null;
                destroyNode(p);
                return true;
            }

            destroyNode(p);
            _root = null;
            return true;
        }

        private void update(Node x)
        {
            x.count = 1;
            if (null != x.l) x.count += x.l.count;
            if (null != x.r) x.count += x.r.count;
        }

        private Node createNode(ref TKey key, ref TValue value)
        {
            Node x;
            if (null != _factory)
                x = _factory.GetObject();
            else
                x = new Node();

            x.key = key;
            x.value = value;
            x.count = 0;
            return x;
        }

        private void destroyNode(Node x)
        {
            x.l = null;
            x.p = null;
            x.r = null;
            x.key = default(TKey);
            x.value = default(TValue);

            if (null != _factory)
                _factory.PutObject(x);
        }
        #endregion
    }
}
