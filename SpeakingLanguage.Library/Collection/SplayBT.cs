using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SpeakingLanguage.Library
{
    [DebuggerDisplay("Count = {Count}")]
    public sealed class SplayBT<TKey, TValue> : IDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
        where TKey : IComparable<TKey>
    {
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator
        {
            private SplayBT<TKey, TValue> tree;
            private Node current;
            private int index;

            public Enumerator(SplayBT<TKey, TValue> t)
            {
                tree = t;
                current = null;
                index = -1;
            }
            
            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    return null == current ? default(KeyValuePair<TKey, TValue>) : new KeyValuePair<TKey, TValue>(current.key, current.value);
                }
            }
            object IEnumerator.Current { get { return this.Current; } }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (null == current)
                    current = tree.first();
                else
                    current = tree.next(current);

                index++;
                return null != current;
            }

            public bool MovePrev()
            {
                if (null == current)
                    current = tree.last();
                else
                    current = tree.prev(current);

                index--;
                return null != current;
            }

            public bool Advance(int ofs)
            {
                index += ofs;
                if (index <= 0 || index >= tree.Count)
                    throw new OverflowException($"[Advance] index overflow - root.count/index:{tree.Count.ToString()}/{index.ToString()}");

                current = tree.find_Kth(index);
                return null != current;
            }

            public void Reset()
            {
                current = null;
                index = -1;
            }
        }

        public struct BidirectEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator
        {
            private SplayBT<TKey, TValue> tree;
            private Node current;

            public BidirectEnumerator(SplayBT<TKey, TValue> t)
            {
                tree = t;
                current = tree._root;
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    return null == current ? default(KeyValuePair<TKey, TValue>) : new KeyValuePair<TKey, TValue>(current.key, current.value);
                }
            }
            object IEnumerator.Current { get { return this.Current; } }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (null == current)
                    current = tree.first();
                else
                    current = tree.next(current);
                return null != current;
            }

            public bool MovePrev()
            {
                if (null == current)
                    current = tree.last();
                else
                    current = tree.prev(current);
                return null != current;
            }
            
            public void Reset()
            {
                current = null;
            }
        }

        class Node
        {
            public Node l, r, p;
            public TKey key;
            public TValue value;
            public int count;

            public override string ToString()
            {
                return string.Format("({0}, {1})", key.ToString(), value.ToString());
            }
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
        private readonly bool _multiKey = true;
        
        public int Count { get { return _root == null ? 0 : _root.count; } }
        public bool IsReadOnly { get { return false; } }
        public ICollection<TKey> Keys { get { throw new NotImplementedException(); } }
        public ICollection<TValue> Values { get { throw new NotImplementedException(); } }

        public TValue this[TKey key]
        {
            get
            {
                var n = find(ref key);
                if (null == n)
                    throw new KeyNotFoundException();
                return n.value;
            }
            set
            {
                insert(ref key, ref value, true);
            }
        }
        
        public static void Test()
        {
            var tree = new SplayBT<int, int>();
            tree.Add(435, 12311);
            tree.Add(34222, 12311);
            tree.Add(33, 412311);
            tree.Add(123566, 233);
            tree.Add(123, 122);
            Console.WriteLine(tree.ToString());
            tree.Remove(123);
            Console.WriteLine(tree.ToString());
            tree.Add(33, 3333);
            tree.TryAdd(33, 9999);
            Console.WriteLine(tree.ToString());
            tree.Add(33, 7777);

            Console.WriteLine("===iterate===");
            var iter = tree.GetEnumerator();
            while (iter.MoveNext())
            {
                Console.WriteLine(iter.Current.ToString());
            }

            Console.WriteLine("===backiterate===");
            while (iter.MovePrev())
            {
                Console.WriteLine(iter.Current.ToString());
            }
            Console.WriteLine("===randomiterate===");
            if (iter.Advance(4))
                Console.WriteLine(iter.Current.ToString());
            else
                Console.WriteLine("fail to advance...");

            tree.Add(992, 77345);
            Console.WriteLine("===BidirectEnumerator===");
            SplayBT<int, int>.BidirectEnumerator biter;
            tree.TryGetEnum(992, out biter);
            while (biter.MoveNext())
            {
                Console.WriteLine(biter.Current.ToString());
            }
            Console.WriteLine("=============");

            if (tree.ContainsKey(123566))
            {
                Console.WriteLine("found: 123566");
            }
            Console.WriteLine(tree.ToString());

            int v;
            if (tree.TryGetValue(34222, out v))
            {
                Console.WriteLine(string.Format("found: 34222, {0}", v.ToString()));
            }
            Console.WriteLine(tree.ToString());

            Console.WriteLine("remove: 34222");
            tree.Remove(34222);
            Console.WriteLine(tree.ToString());

            Console.WriteLine("removeone: 123566");
            tree.RemoveOne(123566, 233);
            Console.WriteLine(tree.ToString());

            Console.WriteLine("removeone: 33");
            tree.RemoveOne(33, 9999);
            Console.WriteLine(tree.ToString());

            Console.WriteLine("delete: 33");
            int key = 33;
            int value = 7777;
            tree.delete(ref key);
            Console.WriteLine(tree.ToString());

            Console.WriteLine("remove: 33");
            tree.Remove(33);
            Console.WriteLine(tree.ToString());

            
        }

        public SplayBT() : this(null, null, true)
        {
        }
        
        public SplayBT(IComparer<TKey> comparer) : this(null, comparer, true)
        {
        }

        public SplayBT(int capacity) : this(createPool(capacity), null, true)
        {
        }

        public SplayBT(int capacity, IComparer<TKey> comparer) : this(createPool(capacity), comparer, true)
        {
        }

        public SplayBT(int capacity, IComparer<TKey> comparer, bool multiKey) : this(createPool(capacity), comparer, multiKey)
        {
        }

        private SplayBT(IFactory factory, IComparer<TKey> comparer, bool multiKey)
        {
            _factory = factory;
            _comparer = comparer;
            _multiKey = multiKey;
        }

        private static IFactory createPool(int sz)
        {
            if (sz == 0)
                return null;
            return new Heap(sz);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("count: {0} / ", Count.ToString());

            sb.Append("elements: ");
            var n = first();
            while (n != null)
            {
                sb.Append(n.ToString());
                sb.Append(' ');
                n = next(n);
            }

            return sb.ToString();
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
            _root = null;
        }

        public bool ContainsKey(TKey key)
        {
            return null != find(ref key);
        }

        public void Add(TKey key, TValue value)
        {
            insert(ref key, ref value, false);
        }

        public void Add(TKey key, TValue value, bool overlap)
        {
            insert(ref key, ref value, overlap);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            return insert(ref key, ref value, false);
        }

        public bool Remove(TKey key)
        {
            bool exist = false;
            while (delete(ref key))
                exist = true;
            return exist;
        }

        public bool RemoveOne(TKey key, TValue value)
        {
            return delete(ref key, ref value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var n = find(ref key);
            if (null == n)
            {
                value = default(TValue);
                return false;
            }

            value = n.value;
            return true;
        }

        public bool TryGetEnum(TKey key, out BidirectEnumerator iter)
        {
            var n = find(ref key);
            if (null == n)
            {
                iter = default(BidirectEnumerator);
                return false;
            }

            iter = new BidirectEnumerator(this);
            return true;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        
        public bool TryFind_Kth(int kth, out TValue value)
        {
            if (_root.count < kth)
                throw new OverflowException($"[Find_Kth] index overflow - root.count/k:{_root.count.ToString()}/{kth.ToString()}");
            
            var x = find_Kth(kth);
            if (null == x)
            {
                value = default(TValue);
                return false;
            }

            value = x.value;
            return true;
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            var key = item.Key;
            var value = item.Value;
            insert(ref key, ref value, false);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var key = item.Key;
            var value = item.Value;
            return null != find(ref key, ref value);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        #region PRIVATE
        private Node next(Node x)
        {
            if (null != x.r)
            {
                x = x.r;
                while (null != x.l)
                    x = x.l;
                return x;
            }

            var p = x.p;
            while (null != p && p.r == x)
            {
                x = p;
                p = p.p;
            }

            return p;
        }
        
        private Node prev(Node x)
        {
            if (null != x.l)
            {
                x = x.l;
                while (null != x.r)
                    x = x.r;
                return x;
            }

            var p = x.p;
            while (null != p && p.l == x)
            {
                x = p;
                p = p.p;
            }

            return p;
        }

        private Node first()
        {
            if (null == _root)
                return null;
            
            Node x = _root;
            while (null != x.l)
                x = x.l;
            
            return x;
        }

        private Node last()
        {
            if (null == _root)
                return null;

            Node x = _root;
            while (null != x.r)
                x = x.r;
            
            return x;
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

        private bool insert(ref TKey key, ref TValue value, bool overlap)
        {
            Node p = _root;
            if (null == p)
            {
                _root = createNode(ref key, ref value);
                return true;
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
                {
                    if (overlap)
                    {
                        p.value = value;
                        return true;
                    }

                    if (!_multiKey)
                        return false;

                    ret = -1;
                }

                if (1 == ret)
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
            return true;
        }

        private Node find(ref TKey key)
        {
            Node p = _root;
            if (null == p)
                return null;

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
                return null;

            return p;
        }

        private Node find(ref TKey key, ref TValue value)
        {
            Node p = _root;
            if (null == p)
                return null;

            IEquatable<TValue> valueEqual;
            while (null != p)
            {
                int ret;
                if (null != _comparer)
                    ret = _comparer.Compare(key, p.key);
                else
                    ret = p.key.CompareTo(key);
                if (ret == 0)
                {
                    valueEqual = p.value as IEquatable<TValue>;
                    if (null != valueEqual && valueEqual.Equals(value))
                        break;

                    if (null != p.r)
                        p = p.r;
                    else if (null != p.l)
                        p = p.l;
                    continue;
                }

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
                return null;

            valueEqual = p.value as IEquatable<TValue>;
            if (null == valueEqual || !valueEqual.Equals(value))
                return null;

            return p;
        }

        private bool delete(ref TKey key)
        {
            if (null == find(ref key))
                return false;

            delete();
            return true;
        }

        private bool delete(ref TKey key, ref TValue value)
        {
            if (null == find(ref key, ref value))
                return false;

            delete();
            return true;
        }

        private void delete()
        {
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
                    update(_root);
                    destroyNode(p);
                    return;
                }
                _root = p.l;
                _root.p = null;
                update(_root);
                destroyNode(p);
                return;
            }

            if (null != p.r)
            {
                _root = p.r;
                _root.p = null;
                update(_root);
                destroyNode(p);
                return;
            }

            destroyNode(p);
            _root = null;
            return;
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
            if (null != _factory)
            {
                x.l = null;
                x.p = null;
                x.r = null;
                _factory.PutObject(x);
            }
        }
        #endregion
    }
}
