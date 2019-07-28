using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SpeakingLanguage.Library
{
    [DebuggerDisplay("Count = {Count}")]
    public sealed class SplayBT<TKey> : SplayBTBase<TKey>, IEnumerable<TKey>, IEnumerable
    {
        sealed class BaseComparer : IComparer<TKey>
        {
            public int Compare(TKey x, TKey y)
            {
                var cmp = y as IComparable<TKey>;
                if (null == cmp)
                    return -1;

                return cmp.CompareTo(x);
            }
        }
        
        public SplayBT() : base(null, new BaseComparer())
        {
        }

        public SplayBT(IComparer<TKey> comparer) : base(null, comparer)
        {
        }

        public SplayBT(int capacity) : base(createPool(capacity), new BaseComparer())
        {
        }

        public SplayBT(int capacity, IComparer<TKey> comparer) : base(createPool(capacity), comparer)
        {
        }
        
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public BidirectEnumerator GetEnumerator(TKey key)
        {
            var n = find(ref key);
            if (null == n)
                return default(BidirectEnumerator);
            
            return new BidirectEnumerator(this);
        }
        
        public bool ContainsKey(TKey key)
        {
            return null != find(ref key);
        }

        public void Add(TKey key)
        {
            insert(ref key, false);
        }

        public void Add(TKey key, bool overlap)
        {
            insert(ref key, overlap);
        }

        public bool TryAdd(TKey key)
        {
            return insert(ref key, false);
        }

        public bool Remove(TKey key)
        {
            return delete(ref key);
        }
        
        public bool TryFind_Kth(int kth, out TKey key)
        {
            if (Count < kth)
                throw new OverflowException($"[Find_Kth] index overflow - root.count/k:{Count.ToString()}/{kth.ToString()}");

            var x = find_Kth(kth);
            if (null == x)
            {
                key = default(TKey);
                return false;
            }

            key = x.key;
            return true;
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    public sealed class SplayBT<TKey, TValue> : SplayBTBase<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
    {
        sealed class BaseComparer : IComparer<KeyValuePair<TKey, TValue>>
        {
            public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
            {
                var cmp = y.Key as IComparable<TKey>;
                if (null == cmp)
                    return -1;

                return cmp.CompareTo(x.Key);
            }
        }

        sealed class WrapedComparer : IComparer<KeyValuePair<TKey, TValue>>
        {
            private IComparer<TKey> _comparer;

            public WrapedComparer(IComparer<TKey> comparer)
            {
                _comparer = comparer;
            }

            public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
            {
                return _comparer.Compare(x.Key, y.Key);
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                var pairNoValue = new KeyValuePair<TKey, TValue>(key, default(TValue));
                var n = find(ref pairNoValue);
                if (null == n)
                    throw new KeyNotFoundException();
                return n.key.Value;
            }
            set
            {
                var pair = new KeyValuePair<TKey, TValue>(key, value);
                insert(ref pair, true);
            }
        }

        public SplayBT() : base(null, new BaseComparer())
        {
        }

        public SplayBT(IComparer<TKey> comparer) : base(null, new WrapedComparer(comparer))
        {
        }

        public SplayBT(IComparer<KeyValuePair<TKey, TValue>> comparer) : base(null, comparer)
        {
        }

        public SplayBT(int capacity) : base(createPool(capacity), new BaseComparer())
        {
        }

        public SplayBT(int capacity, IComparer<TKey> comparer) : base(createPool(capacity), new WrapedComparer(comparer))
        {
        }

        public SplayBT(int capacity, IComparer<KeyValuePair<TKey, TValue>> comparer) : base(createPool(capacity), comparer)
        {
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

        public BidirectEnumerator GetEnumerator(TKey key)
        {
            var pairNoValue = new KeyValuePair<TKey, TValue>(key, default(TValue));
            var n = find(ref pairNoValue);
            if (null == n)
                return default(BidirectEnumerator);
            
            return new BidirectEnumerator(this);
        }
        
        public bool ContainsKey(TKey key)
        {
            var pairNoValue = new KeyValuePair<TKey, TValue>(key, default(TValue));
            return null != find(ref pairNoValue);
        }

        public void Add(TKey key, TValue value)
        {
            var pair = new KeyValuePair<TKey, TValue>(key, value);
            insert(ref pair, false);
        }

        public void Add(TKey key, TValue value, bool overlap)
        {
            var pair = new KeyValuePair<TKey, TValue>(key, value);
            insert(ref pair, overlap);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            var pair = new KeyValuePair<TKey, TValue>(key, value);
            return insert(ref pair, false);
        }

        public bool Remove(TKey key)
        {
            var pairNoValue = new KeyValuePair<TKey, TValue>(key, default(TValue));
            bool exist = false;
            while (delete(ref pairNoValue))
                exist = true;
            return exist;
        }
        
        public bool TryGetValue(TKey key, out TValue value)
        {
            var pairNoValue = new KeyValuePair<TKey, TValue>(key, default(TValue));
            var n = find(ref pairNoValue);
            if (null == n)
            {
                value = default(TValue);
                return false;
            }

            value = n.key.Value;
            return true;
        }
        
        public bool TryFind_Kth(int kth, out TKey key)
        {
            if (Count < kth)
                throw new OverflowException($"[Find_Kth] index overflow - root.count/k:{Count.ToString()}/{kth.ToString()}");

            var x = find_Kth(kth);
            if (null == x)
            {
                key = default(TKey);
                return false;
            }

            key = x.key.Key;
            return true;
        }

        public bool TryFind_Kth(int kth, out TKey key, out TValue value)
        {
            if (Count < kth)
                throw new OverflowException($"[Find_Kth] index overflow - root.count/k:{Count.ToString()}/{kth.ToString()}");

            var x = find_Kth(kth);
            if (null == x)
            {
                key = default(TKey);
                value = default(TValue);
                return false;
            }

            key = x.key.Key;
            value = x.key.Value;
            return true;
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
            Console.WriteLine(tree.ToString());
            tree.Add(33, 7778);
            Console.WriteLine(tree.ToString());
            tree.Add(33, 7777);
            Console.WriteLine(tree.ToString());
            tree.Add(124, 12332);
            tree.Add(3, 12332);

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
            var bidIter = tree.GetEnumerator(33);
            while (bidIter.MoveNext())
            {
                Console.WriteLine(bidIter.Current.ToString());
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

            //Console.WriteLine("removeone: 123566");
            //tree.RemoveOne(123566, 233);
            //Console.WriteLine(tree.ToString());
            //
            //Console.WriteLine("removeone: 33");
            //tree.RemoveOne(33, 9999);
            //Console.WriteLine(tree.ToString());
            //
            //Console.WriteLine("delete: 33");
            //int key = 33;
            //int value = 7777;
            //tree.delete(ref key, ref value);
            //Console.WriteLine(tree.ToString());

            Console.WriteLine("remove: 33");
            tree.Remove(33);
            Console.WriteLine(tree.ToString());
        }
    }

    public abstract class SplayBTBase<TKey>
    {
        public struct Enumerator : IEnumerator<TKey>, IEnumerator
        {
            private SplayBTBase<TKey> tree;
            private Node current;
            private int index;

            public Enumerator(SplayBTBase<TKey> t)
            {
                tree = t;
                current = null;
                index = -1;
            }

            public TKey Current { get { return current.key; } }
            object IEnumerator.Current { get { return this.Current; } }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                index++;
                return EnumeratorHelper.MoveNext(ref current, tree);
            }

            public bool MovePrev()
            {
                index--;
                return EnumeratorHelper.MovePrev(ref current, tree);
            }

            public bool Advance(int ofs)
            {
                return EnumeratorHelper.Advance(ref current, tree, ref index, ofs);
            }

            public void Reset()
            {
                current = null;
                index = -1;
            }
        }

        public struct BidirectEnumerator : IEnumerator<TKey>, IEnumerator
        {
            private SplayBTBase<TKey> tree;
            private Node current;

            public BidirectEnumerator(SplayBTBase<TKey> t)
            {
                tree = t;
                current = t.Root;
                EnumeratorHelper.GetFirstKey(ref current, t);
            }

            public TKey Current { get { return current.key; } }
            object IEnumerator.Current { get { return this.Current; } }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                return EnumeratorHelper.MoveNext(ref current, tree);
            }

            public bool MovePrev()
            {
                return EnumeratorHelper.MovePrev(ref current, tree);
            }

            public void Reset()
            {
                current = null;
            }
        }

        static class EnumeratorHelper
        {
            public static void GetFirstKey(ref Node current, SplayBTBase<TKey> tree)
            {
                if (null == current || null == tree)
                    return;

                var targetKey = current.key;
                while (current != null && tree.compareKeys(ref current.key, ref targetKey) == 0)
                    current = tree.prev(current);
            }

            public static bool MoveNext(ref Node current, SplayBTBase<TKey> tree)
            {
                if (null == tree)
                {
                    current = null;
                    return false;
                }

                if (null == current)
                    current = tree.first();
                else
                    current = tree.next(current);
                return null != current;
            }

            public static bool MovePrev(ref Node current, SplayBTBase<TKey> tree)
            {
                if (null == tree)
                {
                    current = null;
                    return false;
                }

                if (null == current)
                    current = tree.last();
                else
                    current = tree.prev(current);
                return null != current;
            }

            public static bool Advance(ref Node current, SplayBTBase<TKey> tree, ref int index, int ofs)
            {
                if (null == tree)
                {
                    current = null;
                    index = -1;
                    return false;
                }

                index += ofs;
                if (index <= 0 || index >= tree.Count)
                    throw new OverflowException($"[Advance] index overflow - root.count/index:{tree.Count.ToString()}/{index.ToString()}");

                current = tree.find_Kth(index);
                return null != current;
            }
        }

        protected interface IFactory
        {
            Node GetObject();
            void PutObject(Node x);
        }

        protected class Node
        {
            public Node l, r, p;
            public TKey key;
            public int count;

            public override string ToString()
            {
                return string.Format("({0})", key.ToString());
            }
        }

        class Heap : IFactory
        {
            private ConcurrentBag<Node> _pool = new ConcurrentBag<Node>();

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
        
        public int Count { get { return _root == null ? 0 : _root.count; } }
        public bool IsReadOnly { get { return false; } }

        protected Node Root { get { return _root; } }

        protected SplayBTBase(IFactory factory, IComparer<TKey> comparer)
        {
            _factory = factory;
            _comparer = comparer;
        }

        protected static IFactory createPool(int sz)
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

        public void Clear()
        {
            _root = null;
        }

        #region PROTECTED
        protected Node next(Node x)
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

        protected Node prev(Node x)
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

        protected Node first()
        {
            if (null == _root)
                return null;
            
            Node x = _root;
            while (null != x.l)
                x = x.l;
            
            return x;
        }

        protected Node last()
        {
            if (null == _root)
                return null;

            Node x = _root;
            while (null != x.r)
                x = x.r;
            
            return x;
        }

        protected Node find_Kth(int k)
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

        protected bool insert(ref TKey key, bool overlap)
        {
            Node p = _root;
            if (null == p)
            {
                _root = createNode(ref key);
                return true;
            }

            bool left;
            while (true)
            {
                int ret = compareKeys(ref key, ref p.key);
                if (0 == ret)
                {
                    if (overlap)
                    {
                        p.key = key;
                        return true;
                    }

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

            var x = createNode(ref key);
            if (left)
                p.l = x;
            else
                p.r = x;
            x.p = p;
            splay(x);
            return true;
        }

        protected Node find(ref TKey key)
        {
            Node p = _root;
            if (null == p)
                return null;

            while (null != p)
            {
                int ret = compareKeys(ref key, ref p.key);
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

            bool equal = compareKeys(ref key, ref p.key) == 0;
            if (!equal)
                return null;

            return p;
        }
        
        protected bool delete(ref TKey key)
        {
            if (null == find(ref key))
                return false;

            delete();
            return true;
        }

        protected int compareKeys(ref TKey k1, ref TKey k2)
        {
            return _comparer.Compare(k1, k2);
        }
        #endregion

        #region PRIVATE
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
                    destroyNode(p);
                    return;
                }
                _root = p.l;
                _root.p = null;
                destroyNode(p);
                return;
            }

            if (null != p.r)
            {
                _root = p.r;
                _root.p = null;
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

        private Node createNode(ref TKey key)
        {
            Node x;
            if (null != _factory)
                x = _factory.GetObject();
            else
                x = new Node();

            x.key = key;
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
