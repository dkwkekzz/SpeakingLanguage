using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Library
{
    internal unsafe struct umnFactoryPtr
    {
        public umnFactoryPtr* prev;
        public umnFactoryPtr* next;
    }

    public unsafe struct umnFactory<TAllocator, T> : IumnFactory<T>
        where TAllocator : unmanaged, IumnAllocator
        where T : unmanaged
    {
        public struct Enumerator
        {
            private umnFactoryPtr* _head;
            private umnFactoryPtr* _current;

            internal Enumerator(umnFactoryPtr* head)
            {
                _head = head;
                _current = null;
            }

            public T* Current => (T*)(_current + sizeof(umnFactoryPtr));

            public bool MoveNext()
            {
                if (_current == null)
                    _current = _head;
                else
                    _current = _current->prev;

                return _current != null;
            }

            public void Reset()
            {
                _current = null;
            }
        }

        private readonly TAllocator* _allocator;
        private readonly int _szNode;
        private umnFactoryPtr* _used;
        private umnFactoryPtr* _unused;

        public umnFactory(TAllocator* allocator, int capacity = 0)
        {
            _allocator = allocator;
            _szNode = sizeof(T);
            _used = null;
            _unused = null;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_used);
        }

        public T* GetObject()
        {
            umnFactoryPtr* cur = null;
            if (_unused != null)
            {
                cur = _getTail(ref _unused);
            }
            else
            {
                var chk = _allocator->Alloc(_szNode + sizeof(umnFactoryPtr));
                cur = (umnFactoryPtr*)chk->Ptr;
            }
            
            _putTail(ref _used, ref cur);

            return (T*)(cur + sizeof(umnFactoryPtr));
        }

        public void PutObject(T* x)
        {
            var cur = (umnFactoryPtr*)((IntPtr)x - sizeof(umnFactoryPtr));
            _delete(ref _used, ref cur);
            _putTail(ref _unused, ref cur);
        }
        
        private void _putTail(ref umnFactoryPtr* prev, ref umnFactoryPtr* cur)
        {
            if (prev != null)
                prev->next = cur;
            cur->prev = prev;
            cur->next = null;
            prev = cur;
        }

        private umnFactoryPtr* _getTail(ref umnFactoryPtr* head)
        {
            var ret = head;
            head = head->prev;
            return ret;
        }

        private void _delete(ref umnFactoryPtr* head, ref umnFactoryPtr* cur)
        {
            var prev = cur->prev;
            var next = cur->next;
            if (prev != null)
                prev->next = next;
            if (next != null)
                next->prev = prev;
            else
                head = prev;
        }
    }
}
