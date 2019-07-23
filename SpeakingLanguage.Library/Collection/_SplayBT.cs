using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Library
{
    unsafe struct Node
    {
        public Node* l, r, p;
        public int key;
        public int value;
        public int count;
    }
    
    public unsafe class _SplayBT<TKey, TValue> where TKey : IComparable<TKey>
    { 
        //class Pair
        //{
        //    public TKey key;
        //    public TValue value;
        //}

        //class Factory
        //{
        //    public Pair GetObject()
        //    {
        //        return new Pair();
        //    }

        //    public void PutObject(Pair pair)
        //    {

        //    }
        //}

        //private Factory _factory = new Factory();

        private Node[] _nodes;
        private int _nIndex;

        private Node* _root;

        public _SplayBT(int capacity = 1 << 10)
        {
            _root = null;
            _nodes = new Node[capacity];
        }

        //public void Insert(int key, TValue value)
        //{
        //    GCHandle gch = GCHandle.Alloc(key);
        //    var ptr = (void*)GCHandle.ToIntPtr(gch);

        //}

        public void Insert(int key, int value)
        {
            insert(key, value);
        }

        public bool Delete(TKey key)
        {
            return false;
        }

        public bool Find(int key)
        {
            return find(key) != -1;
        }

        #region PRIVATE
        private Node* createNode()
        {
            fixed (Node* p = &_nodes[_nIndex++])
            {
                return p;
            }
        }

        private void destroyNode(Node* x)
        {
            x->l = null;
            x->p = null;
            x->r = null;
        }

        private void rotate(Node* x)
        {
            Node* p = x->p;
            Node* b;
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

            Node* g = x->p;
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
        }

        private void splay(Node* x)
        {
            while (null != x->p)
            {
                Node* p = x->p;
                Node* g = p->p;
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

        private void insert(int key, int value)
        {
            Node* p = _root;
            Node** pp;
            if (null == p)
            {
                var x = createNode();
                _root = x;
                x->l = x->r = x->p = null;
                x->key = key;
                return;
            }

            while (true)
            {
                if (key == p->key)
                    return;
                else if(key < p->key)
                {
                    if (null == p->l)
                    {
                        pp = &p->l;
                        break;
                    }
                    p = p->l;
                }
                else
                {
                    if (null == p->r)
                    {
                        pp = &p->r;
                        break;
                    }
                    p = p->r;
                }
            }

            {
                var x = createNode();
                *pp = x;
                x->l = x->r = x->p = null;
                x->p = p;
                x->key = key;
                splay(x);
            }
        }

        private int find(int key)
        {
            Node* p = _root;
            if (null == p)
                return -1;

            while (null != p)
            {
                if (key == p->key)
                    break;
                if (key < p->key)
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
            return key == p->key ? p->value : -1;
        }

        private void delete(int key)
        {
            if (-1 == find(key))
                return;

            Node* p = _root;
            if (null != p->l)
            {
                if (null != p->r)
                {
                    _root = p->l;
                    _root->p = null;
                    Node* x = _root;
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
        }

        private void update(Node* x)
        {
            x->count = 1;
            if (null != x->l) x->count += x->l->count;
            if (null != x->r) x->count += x->r->count;
        }
        #endregion
    }
}
