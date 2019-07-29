using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SpeakingLanguage.Library
{
    [DebuggerDisplay("Count = {Count}")]
    public unsafe sealed class umnSplayBT<TAllocator, TKey> : umnSplayBTBase<TAllocator, TKey>
        where TAllocator : unmanaged, IumnAllocator
        where TKey : unmanaged
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
        
        public umnSplayBT(TAllocator* allocator, int capacity) : base(allocator, capacity, new BaseComparer())
        {
        }

        public umnSplayBT(TAllocator* allocator, int capacity, IComparer<TKey> comparer) : base(allocator, capacity, comparer)
        {
        }
        
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public FastEnumerator GetFastEnumerator(sbtNode** stackPtr)
        {
            return new FastEnumerator(this, stackPtr);
        }

        public BidirectEnumerator GetEnumerator(TKey key)
        {
            return GetEnumerator(&key);
        }

        public BidirectEnumerator GetEnumerator(TKey* key)
        {
            var n = find(key);
            if (null == n)
                return default(BidirectEnumerator);
            
            return new BidirectEnumerator(this);
        }

        public bool ContainsKey(TKey key)
        {
            return null != find(&key);
        }

        public bool ContainsKey(TKey* key)
        {
            return null != find(key);
        }
        
        public void Add(TKey* key)
        {
            insert(key, false);
        }

        public void Add(TKey* key, bool overlap)
        {
            insert(key, overlap);
        }
        
        public bool TryAdd(TKey* key)
        {
            return insert(key, false);
        }

        public bool Remove(TKey key)
        {
            return delete(&key);
        }

        public bool Remove(TKey* key)
        {
            return delete(key);
        }

        public bool TryGetValue(TKey key, out TKey* value)
        {
            var n = find(&key);
            if (null == n)
            {
                value = null;
                return false;
            }

            value = (TKey*)n->key;
            return true;
        }

        public bool TryGetValue(TKey* key, out TKey* value)
        {
            var n = find(key);
            if (null == n)
            {
                value = null;
                return false;
            }
        
            value = (TKey*)n->key;
            return true;
        }
        
        public bool TryFind_Kth(int kth, out TKey* key)
        {
            if (Count < kth)
                throw new OverflowException($"[Find_Kth] index overflow - root->count/k:{Count.ToString()}/{kth.ToString()}");

            var x = find_Kth(kth);
            if (null == x)
            {
                key = null;
                return false;
            }

            key = (TKey*)x->key;
            return true;
        }
    }

    //[DebuggerDisplay("Count = {Count}")]
    //public sealed class StSplayBT<TKey, TValue> : StSplayBTBase<SplayPair<TKey, TValue>>
    //    where TKey : unmanaged
    //    where TValue : unmanaged
    //{
    //    sealed class BaseComparer : IComparer<SplayPair<TKey, TValue>>
    //    {
    //        public int Compare(SplayPair<TKey, TValue> x, SplayPair<TKey, TValue> y)
    //        {
    //            var cmp = y.Key as IComparable<TKey>;
    //            if (null == cmp)
    //                return -1;
    //
    //            return cmp.CompareTo(x.Key);
    //        }
    //    }
    //
    //    sealed class WrapedComparer : IComparer<SplayPair<TKey, TValue>>
    //    {
    //        private IComparer<TKey> _comparer;
    //
    //        public WrapedComparer(IComparer<TKey> comparer)
    //        {
    //            _comparer = comparer;
    //        }
    //
    //        public int Compare(SplayPair<TKey, TValue> x, SplayPair<TKey, TValue> y)
    //        {
    //            return _comparer.Compare(x.Key, y.Key);
    //        }
    //    }
    //
    //    public TValue this[TKey key]
    //    {
    //        get
    //        {
    //            var pairNoValue = new SplayPair<TKey, TValue>(key, default(TValue));
    //            var n = find(ref pairNoValue);
    //            if (null == n)
    //                throw new KeyNotFoundException();
    //            return n.key.Value;
    //        }
    //        set
    //        {
    //            var pair = new SplayPair<TKey, TValue>(key, value);
    //            insert(ref pair, true);
    //        }
    //    }
    //
    //    public StSplayBT() : base(null, new BaseComparer())
    //    {
    //    }
    //
    //    public StSplayBT(IComparer<TKey> comparer) : base(null, new WrapedComparer(comparer))
    //    {
    //    }
    //
    //    public StSplayBT(IComparer<SplayPair<TKey, TValue>> comparer) : base(null, comparer)
    //    {
    //    }
    //
    //    public StSplayBT(int capacity) : base(createPool(capacity), new BaseComparer())
    //    {
    //    }
    //
    //    public StSplayBT(int capacity, IComparer<TKey> comparer) : base(createPool(capacity), new WrapedComparer(comparer))
    //    {
    //    }
    //
    //    public StSplayBT(int capacity, IComparer<SplayPair<TKey, TValue>> comparer) : base(createPool(capacity), comparer)
    //    {
    //    }
    //
    //    public Enumerator GetEnumerator()
    //    {
    //        return new Enumerator(this);
    //    }
    //
    //    IEnumerator<SplayPair<TKey, TValue>> IEnumerable<SplayPair<TKey, TValue>>.GetEnumerator()
    //    {
    //        return this.GetEnumerator();
    //    }
    //
    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return this.GetEnumerator();
    //    }
    //
    //    public BidirectEnumerator GetEnumerator(TKey key)
    //    {
    //        var pairNoValue = new SplayPair<TKey, TValue>(key, default(TValue));
    //        var n = find(ref pairNoValue);
    //        if (null == n)
    //            return default(BidirectEnumerator);
    //        
    //        return new BidirectEnumerator(this);
    //    }
    //    
    //    public bool ContainsKey(TKey key)
    //    {
    //        var pairNoValue = new SplayPair<TKey, TValue>(key, default(TValue));
    //        return null != find(ref pairNoValue);
    //    }
    //
    //    public void Add(TKey key, TValue value)
    //    {
    //        var pair = new SplayPair<TKey, TValue>(key, value);
    //        insert(ref pair, false);
    //    }
    //
    //    public void Add(TKey key, TValue value, bool overlap)
    //    {
    //        var pair = new SplayPair<TKey, TValue>(key, value);
    //        insert(ref pair, overlap);
    //    }
    //
    //    public bool TryAdd(TKey key, TValue value)
    //    {
    //        var pair = new SplayPair<TKey, TValue>(key, value);
    //        return insert(ref pair, false);
    //    }
    //
    //    public bool Remove(TKey key)
    //    {
    //        var pairNoValue = new SplayPair<TKey, TValue>(key, default(TValue));
    //        bool exist = false;
    //        while (delete(ref pairNoValue))
    //            exist = true;
    //        return exist;
    //    }
    //    
    //    public bool TryGetValue(TKey key, out TValue value)
    //    {
    //        var pairNoValue = new SplayPair<TKey, TValue>(key, default(TValue));
    //        var n = find(ref pairNoValue);
    //        if (null == n)
    //        {
    //            value = default(TValue);
    //            return false;
    //        }
    //
    //        value = n.key.Value;
    //        return true;
    //    }
    //    
    //    public bool TryFind_Kth(int kth, out TKey key)
    //    {
    //        if (Count < kth)
    //            throw new OverflowException($"[Find_Kth] index overflow - root->count/k:{Count.ToString()}/{kth.ToString()}");
    //
    //        var x = find_Kth(kth);
    //        if (null == x)
    //        {
    //            key = default(TKey);
    //            return false;
    //        }
    //
    //        key = x.key.Key;
    //        return true;
    //    }
    //
    //    public bool TryFind_Kth(int kth, out TKey key, out TValue value)
    //    {
    //        if (Count < kth)
    //            throw new OverflowException($"[Find_Kth] index overflow - root->count/k:{Count.ToString()}/{kth.ToString()}");
    //
    //        var x = find_Kth(kth);
    //        if (null == x)
    //        {
    //            key = default(TKey);
    //            value = default(TValue);
    //            return false;
    //        }
    //
    //        key = x.key.Key;
    //        value = x.key.Value;
    //        return true;
    //    }
    //
    //    public static void Test()
    //    {
    //        var tree = new StSplayBT<int, int>();
    //        tree.Add(435, 12311);
    //        tree.Add(34222, 12311);
    //        tree.Add(33, 412311);
    //        tree.Add(123566, 233);
    //        tree.Add(123, 122);
    //        Console.WriteLine(tree.ToString());
    //        tree->remove(123);
    //        Console.WriteLine(tree.ToString());
    //        tree.Add(33, 3333);
    //        tree.TryAdd(33, 9999);
    //        Console.WriteLine(tree.ToString());
    //        tree.Add(33, 7777);
    //        Console.WriteLine(tree.ToString());
    //        tree.Add(33, 7778);
    //        Console.WriteLine(tree.ToString());
    //        tree.Add(33, 7777);
    //        Console.WriteLine(tree.ToString());
    //        tree.Add(124, 12332);
    //        tree.Add(3, 12332);
    //
    //        Console.WriteLine("===iterate===");
    //        var iter = tree.GetEnumerator();
    //        while (iter.MoveNext())
    //        {
    //            Console.WriteLine(iter.Current.ToString());
    //        }
    //
    //        Console.WriteLine("===backiterate===");
    //        while (iter.MovePrev())
    //        {
    //            Console.WriteLine(iter.Current.ToString());
    //        }
    //        Console.WriteLine("===randomiterate===");
    //        if (iter.Advance(4))
    //            Console.WriteLine(iter.Current.ToString());
    //        else
    //            Console.WriteLine("fail to advance...");
    //
    //        tree.Add(992, 77345);
    //        Console.WriteLine("===BidirectEnumerator===");
    //        var bidIter = tree.GetEnumerator(33);
    //        while (bidIter.MoveNext())
    //        {
    //            Console.WriteLine(bidIter.Current.ToString());
    //        }
    //        Console.WriteLine("=============");
    //
    //        if (tree.ContainsKey(123566))
    //        {
    //            Console.WriteLine("found: 123566");
    //        }
    //        Console.WriteLine(tree.ToString());
    //
    //        int v;
    //        if (tree.TryGetValue(34222, out v))
    //        {
    //            Console.WriteLine(string.Format("found: 34222, {0}", v.ToString()));
    //        }
    //        Console.WriteLine(tree.ToString());
    //
    //        Console.WriteLine("remove: 34222");
    //        tree->remove(34222);
    //        Console.WriteLine(tree.ToString());
    //
    //        //Console.WriteLine("removeone: 123566");
    //        //tree->removeOne(123566, 233);
    //        //Console.WriteLine(tree.ToString());
    //        //
    //        //Console.WriteLine("removeone: 33");
    //        //tree->removeOne(33, 9999);
    //        //Console.WriteLine(tree.ToString());
    //        //
    //        //Console.WriteLine("delete: 33");
    //        //int key = 33;
    //        //int value = 7777;
    //        //tree.delete(ref key, ref value);
    //        //Console.WriteLine(tree.ToString());
    //
    //        Console.WriteLine("remove: 33");
    //        tree->remove(33);
    //        Console.WriteLine(tree.ToString());
    //    }
    //}

    public unsafe struct sbtNode
    {
        public sbtNode* l, r, p;
        public void* key;
        public int count;
    }

    public unsafe abstract class umnSplayBTBase<TAllocator, TKey>
        where TAllocator : unmanaged, IumnAllocator
        where TKey : unmanaged
    {
        public struct FastEnumerator
        {
            private umnSplayBTBase<TAllocator, TKey> tree;
            private sbtNode** current;
            private sbtNode** tempBuffer;

            public FastEnumerator(umnSplayBTBase<TAllocator, TKey> t, sbtNode** stackPtr)
            {
                tree = t;
                current = null;
                tempBuffer = stackPtr;
            }

            public TKey* Current { get { return (TKey*)(*current)->key; } }

            public bool MoveNext()
            {
                if (current == null)
                {
                    *tempBuffer = tree._root;
                    current = tempBuffer;
                }
                else
                    current++;

                if ((*current) == null)
                    return false;

                if ((*current)->l != null)
                    *(++tempBuffer) = (*current)->l;
                if ((*current)->r != null)
                    *(++tempBuffer) = (*current)->r;
                
                return true;
            }
        }

        public struct Enumerator
        {
            private umnSplayBTBase<TAllocator, TKey> tree;
            private sbtNode* current;
            private int index;

            public Enumerator(umnSplayBTBase<TAllocator, TKey> t)
            {
                tree = t;
                current = null;
                index = -1;
            }

            public TKey* Current { get { return (TKey*)current->key; } }
            
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

        public struct BidirectEnumerator
        {
            private umnSplayBTBase<TAllocator, TKey> tree;
            private sbtNode* current;

            public BidirectEnumerator(umnSplayBTBase<TAllocator, TKey> t)
            {
                tree = t;
                current = t._root;
                EnumeratorHelper.GetFirstKey(ref current, t);
            }

            public TKey* Current { get { return (TKey*)current->key; } }
            
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
            public static void GetFirstKey(ref sbtNode* current, umnSplayBTBase<TAllocator, TKey> tree)
            {
                if (null == current || null == tree)
                    return;

                var targetKey = current->key;
                while (current != null && tree.compareKeys((TKey*)current->key, (TKey*)targetKey) == 0)
                    current = tree.prev(current);
            }

            public static bool MoveNext(ref sbtNode* current, umnSplayBTBase<TAllocator, TKey> tree)
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

            public static bool MovePrev(ref sbtNode* current, umnSplayBTBase<TAllocator, TKey> tree)
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

            public static bool Advance(ref sbtNode* current, umnSplayBTBase<TAllocator, TKey> tree, ref int index, int ofs)
            {
                if (null == tree)
                {
                    current = null;
                    index = -1;
                    return false;
                }

                index += ofs;
                if (index <= 0 || index >= tree.Count)
                    throw new OverflowException($"[Advance] index overflow - root->count/index:{tree.Count.ToString()}/{index.ToString()}");

                current = tree.find_Kth(index);
                return null != current;
            }
        }

        private umnFactory<TAllocator, sbtNode> _factory;
        private IComparer<TKey> _comparer;
        private sbtNode* _root;
        
        public int Count { get { return _root == null ? 0 : _root->count; } }
        public bool IsReadOnly { get { return false; } }
        
        protected umnSplayBTBase(TAllocator* allocator, int capacity, IComparer<TKey> comparer)
        {
            _factory = new umnFactory<TAllocator, sbtNode>(allocator, capacity);
            _comparer = comparer;
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("count: {0} / ", Count.ToString());

            sb.Append("elements: ");
            var n = first();
            while (n != null)
            {
                sb.Append((*(TKey*)n->key).ToString());
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
        protected sbtNode* next(sbtNode* x)
        {
            if (null != x->r)
            {
                x = x->r;
                while (null != x->l)
                    x = x->l;
                return x;
            }

            var p = x->p;
            while (null != p && p->r == x)
            {
                x = p;
                p = p->p;
            }

            return p;
        }

        protected sbtNode* prev(sbtNode* x)
        {
            if (null != x->l)
            {
                x = x->l;
                while (null != x->r)
                    x = x->r;
                return x;
            }

            var p = x->p;
            while (null != p && p->l == x)
            {
                x = p;
                p = p->p;
            }

            return p;
        }

        protected sbtNode* first()
        {
            if (null == _root)
                return null;
            
            sbtNode* x = _root;
            while (null != x->l)
                x = x->l;
            
            return x;
        }

        protected sbtNode* last()
        {
            if (null == _root)
                return null;

            sbtNode* x = _root;
            while (null != x->r)
                x = x->r;
            
            return x;
        }

        protected sbtNode* find_Kth(int k)
        {
            sbtNode* x = _root;
            while (true)
            {
                while (null != x->l && x->l->count > k)
                    x = x->l;
                if (null != x->l)
                    k -= x->l->count;
                if (k-- == 0)
                    break;
                x = x->r;
            }

            splay(x);

            return _root;
        }

        protected bool insert(TKey* key, bool overlap)
        {
            sbtNode* p = _root;
            if (null == p)
            {
                _root = createNode(key);
                return true;
            }

            bool left;
            while (true)
            {
                int ret = compareKeys(key, (TKey*)p->key);
                if (0 == ret)
                {
                    if (overlap)
                    {
                        p->key = key;
                        return true;
                    }

                    ret = -1;
                }

                if (1 == ret)
                {
                    if (null == p->l)
                    {
                        left = true;
                        break;
                    }
                    p = p->l;
                }
                else
                {
                    if (null == p->r)
                    {
                        left = false;
                        break;
                    }
                    p = p->r;
                }
            }

            var x = createNode(key);

            if (left)
                p->l = x;
            else
                p->r = x;
            x->p = p;
            splay(x);
            return true;
        }

        protected sbtNode* find(TKey* key)
        {
            sbtNode* p = _root;
            if (null == p)
                return null;

            while (null != p)
            {
                int ret = compareKeys(key, (TKey*)p->key);
                if (ret == 0)
                    break;
                if (ret == 1)
                {
                    if (null == p->l)
                        break;
                    p = p->l;
                }
                else
                {
                    if (null == p->r)
                        break;
                    p = p->r;
                }
            }

            splay(p);

            bool equal = compareKeys(key, (TKey*)p->key) == 0;
            if (!equal)
                return null;

            return p;
        }
        
        protected bool delete(TKey* key)
        {
            if (null == find(key))
                return false;

            delete();
            return true;
        }

        protected int compareKeys(TKey* k1, TKey* k2)
        {
            return _comparer.Compare(*k1, *k2);
        }
        #endregion

        #region PRIVATE
        private void delete()
        {
            sbtNode* p = _root;
            if (null != p->l)
            {
                if (null != p->r)
                {
                    _root = p->l;
                    _root->p = null;
                    sbtNode* x = _root;
                    while (null != x->r)
                        x = x->r;
                    x->r = p->r;
                    p->r->p = x;
                    splay(x);
                    destroyNode(p);
                    return;
                }
                _root = p->l;
                _root->p = null;
                destroyNode(p);
                return;
            }

            if (null != p->r)
            {
                _root = p->r;
                _root->p = null;
                destroyNode(p);
                return;
            }

            destroyNode(p);
            _root = null;
            return;
        }

        private void rotate(sbtNode* x)
        {
            sbtNode* p = x->p;
            sbtNode* b;
            if (x == p->l)
            {
                p->l = b = x->r;
                x->r = p;
            }
            else
            {
                p->r = b = x->l;
                x->l = p;
            }

            x->p = p->p;
            p->p = x;
            if (null != b)
                b->p = p;

            sbtNode* g = x->p;
            if (null != g)
            {
                if (p == g->l)
                    g->l = x;
                else
                    g->r = x;
            }
            else
            {
                _root = x;
            }

            update(p);
            update(x);
        }

        private void splay(sbtNode* x)
        {
            if (null == x->p)
            {
                update(x);
                return;
            }

            while (null != x->p)
            {
                sbtNode* p = x->p;
                sbtNode* g = p->p;
                if (null != g)
                {
                    bool zigzig = (p->l == x) == (g->l == p);
                    if (zigzig)
                        rotate(p);
                    else
                        rotate(x);
                }

                rotate(x);
            }
        }

        private void update(sbtNode* x)
        {
            x->count = 1;
            if (null != x->l) x->count += x->l->count;
            if (null != x->r) x->count += x->r->count;
        }

        private sbtNode* createNode(TKey* key)
        {
            sbtNode* x = _factory.GetObject();
            x->key = key;
            x->count = 0;
            return x;
        }

        private void destroyNode(sbtNode* x)
        {
            x->l = null;
            x->p = null;
            x->r = null;
            _factory.PutObject(x);
        }
        #endregion
    }
}
