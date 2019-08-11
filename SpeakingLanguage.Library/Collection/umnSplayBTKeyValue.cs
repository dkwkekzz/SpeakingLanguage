using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SpeakingLanguage.Library
{
    public unsafe struct sbtPairNode
    {
        public sbtPairNode* l, r, p;
        public void* key;
        public void* value;
        public int count;
    }

    [DebuggerDisplay("Count = {Count}")]
    public unsafe struct umnSplayBT<TAllocator, TComparer, TKey, TValue>
        where TAllocator : unmanaged, IumnAllocator
        where TComparer : unmanaged, IumnComparer<TKey>
        where TKey : unmanaged
        where TValue : unmanaged
    {
        public struct FastEnumerator
        {
            private sbtPairNode* root;
            private sbtPairNode** current;
            private sbtPairNode** tempBuffer;

            public FastEnumerator(sbtPairNode* r, sbtPairNode** stackPtr)
            {
                root = r;
                current = null;
                tempBuffer = stackPtr;
            }

            public sbtPairNode* Current { get { return *current; } }

            public bool MoveNext()
            {
                if (current == null)
                {
                    *tempBuffer = root;
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
            private sbtPairNode* _root;
            private sbtPairNode* _current;

            public Enumerator(sbtPairNode* root, sbtPairNode* cur)
            {
                _root = root;
                _current = cur;
            }

            public TKey* CurrentKey { get { return (TKey*)_current->key; } }
            public TValue* Current { get { return (TValue*)_current->value; } }
            
            public bool MoveNext()
            {
                if (null == _current)
                    _current = umnSplayBTEnumerateHelper.first(_root);
                else
                    _current = umnSplayBTEnumerateHelper.next(_current);
                return null != _current;
            }

            public bool MovePrev()
            {
                if (null == _current)
                    _current = umnSplayBTEnumerateHelper.last(_root);
                else
                    _current = umnSplayBTEnumerateHelper.prev(_current);
                return null != _current;
            }

            public void Reset()
            {
                _current = null;
            }
        }
        
        struct umnSplayBTEnumerateHelper
        {
            public static sbtPairNode* next(sbtPairNode* x)
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

            public static sbtPairNode* prev(sbtPairNode* x)
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

            public static sbtPairNode* first(sbtPairNode* root)
            {
                if (null == root)
                    return null;

                sbtPairNode* x = root;
                while (null != x->l)
                    x = x->l;

                return x;
            }

            public static sbtPairNode* last(sbtPairNode* root)
            {
                if (null == root)
                    return null;

                sbtPairNode* x = root;
                while (null != x->r)
                    x = x->r;

                return x;
            }
        }

        //private umnFactory<TAllocator, sbtPairNode> _factory;
        private TAllocator* _allocator;
        private TComparer _comparer;
        private sbtPairNode* _root;
        
        public int Count { get { return _root == null ? 0 : _root->count; } }
        public bool IsReadOnly { get { return false; } }
        
        public TValue* this[TKey* key]
        {
            get
            {
                var n = find(key);
                if (null == n)
                    return null;

                return (TValue*)n->value;
            }
        }

        public TValue* this[TKey key]
        {
            get
            {
                var n = find(&key);
                if (null == n)
                    return null;

                return (TValue*)n->value;
            }
        }

        public umnSplayBT(TAllocator* allocator, int capacity = 0)
        {
            //_factory = new umnFactory<TAllocator, sbtPairNode>(allocator, capacity);
            _allocator = allocator;
            _comparer = new TComparer();
            _root = null;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("count: {0} / ", Count.ToString());

            sb.Append("elements: ");

            var iter = new Enumerator(_root, null);
            while (iter.MoveNext())
            {
                sb.Append((*iter.Current).ToString());
                sb.Append(' ');
            }

            return sb.ToString();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_root, null);
        }
        
        public FastEnumerator GetFastEnumerator(sbtPairNode** stackPtr)
        {
            return new FastEnumerator(_root, stackPtr);
        }

        public Enumerator GetEnumerator(TKey key)
        {
            return GetEnumerator(&key);
        }

        public Enumerator GetEnumerator(TKey* key)
        {
            var n = find(key);
            if (null == n)
                return default(Enumerator);
            
            while (n != null && _comparer.Compare((TKey*)n->key, key) == 0)
                n = umnSplayBTEnumerateHelper.prev(n);

            return new Enumerator(_root, n);
        }

        public bool ContainsKey(TKey key)
        {
            return null != find(&key);
        }

        public bool ContainsKey(TKey* key)
        {
            return null != find(key);
        }

        public void Add(TKey key, TValue* value)
        {
            insert(&key, value, false);
        }

        public void Add(TKey* key, TValue* value)
        {
            insert(key, value, false);
        }

        public void Add(TKey* key, TValue* value, bool overlap)
        {
            insert(key, value, overlap);
        }

        public bool TryAdd(TKey key, TValue* value)
        {
            return insert(&key, value, false);
        }

        public bool TryAdd(TKey* key, TValue* value)
        {
            return insert(key, value, false);
        }

        public bool Remove(TKey key)
        {
            return delete(&key);
        }

        public bool Remove(TKey* key)
        {
            return delete(key);
        }

        public bool TryGetValue(TKey key, out TValue* value)
        {
            var n = find(&key);
            if (null == n)
            {
                value = null;
                return false;
            }

            value = (TValue*)n->value;
            return true;
        }

        public bool TryGetValue(TKey* key, out TValue* value)
        {
            var n = find(key);
            if (null == n)
            {
                value = null;
                return false;
            }

            value = (TValue*)n->value;
            return true;
        }

        public bool TryFind_Kth(int kth, out TKey* key, out TValue* value)
        {
            if (Count < kth)
                throw new OverflowException($"[Find_Kth] index overflow - root->count/k:{Count.ToString()}/{kth.ToString()}");

            var x = find_Kth(kth);
            if (null == x)
            {
                key = null;
                value = null;
                return false;
            }

            key = (TKey*)x->key;
            value = (TValue*)x->value;
            return true;
        }

        public void Clear()
        {
            _root = null;
        }

        #region PRIVATE
        private sbtPairNode* find_Kth(int k)
        {
            sbtPairNode* x = _root;
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

        private sbtPairNode* find(TKey* key)
        {
            sbtPairNode* p = _root;
            if (null == p)
                return null;

            while (null != p)
            {
                int ret = _comparer.Compare(key, (TKey*)p->key);
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

            bool equal = _comparer.Compare(key, (TKey*)p->key) == 0;
            if (!equal)
                return null;

            return p;
        }
        
        private bool insert(TKey* key, TValue* value, bool overlap)
        {
            sbtPairNode* p = _root;
            if (null == p)
            {
                _root = createNode(key, value);
                return true;
            }

            bool left;
            while (true)
            {
                int ret = _comparer.Compare(key, (TKey*)p->key);
                if (0 == ret)
                {
                    if (overlap)
                    {
                        p->key = key;
                        p->value = value;
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

            var x = createNode(key, value);

            if (left)
                p->l = x;
            else
                p->r = x;
            x->p = p;
            splay(x);
            return true;
        }

        private bool delete(TKey* key)
        {
            if (null == find(key))
                return false;

            sbtPairNode* p = _root;
            if (null != p->l)
            {
                if (null != p->r)
                {
                    _root = p->l;
                    _root->p = null;
                    sbtPairNode* x = _root;
                    while (null != x->r)
                        x = x->r;
                    x->r = p->r;
                    p->r->p = x;
                    splay(x);
                    destroyNode(p);
                    return true;
                }
                _root = p->l;
                _root->p = null;
                destroyNode(p);
                return true;
            }

            if (null != p->r)
            {
                _root = p->r;
                _root->p = null;
                destroyNode(p);
                return true;
            }

            destroyNode(p);
            _root = null;
            return true;
        }

        private void rotate(sbtPairNode* x)
        {
            sbtPairNode* p = x->p;
            sbtPairNode* b;
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

            sbtPairNode* g = x->p;
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

        private void splay(sbtPairNode* x)
        {
            if (null == x->p)
            {
                update(x);
                return;
            }

            while (null != x->p)
            {
                sbtPairNode* p = x->p;
                sbtPairNode* g = p->p;
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

        private void update(sbtPairNode* x)
        {
            x->count = 1;
            if (null != x->l) x->count += x->l->count;
            if (null != x->r) x->count += x->r->count;
        }

        private sbtPairNode* createNode(TKey* key, TValue* value)
        {
            var chk = _allocator->Alloc(umnSize.sbtPairNode);
            chk->typeHandle = umnTypeHandle.sbtPairNode;

            var x = umnChunk.GetPtr<sbtPairNode>(chk);
            x->key = key;
            x->value = value;
            x->count = 0;
            return x;
        }

        private void destroyNode(sbtPairNode* x)
        {
            x->l = null;
            x->p = null;
            x->r = null;

            var chk = umnChunk.GetChunk(x);
            chk->Disposed = true;
        }
        #endregion
    }
}
