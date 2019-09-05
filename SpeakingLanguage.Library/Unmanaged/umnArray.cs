using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Library
{
    public unsafe struct umnArray<T> : IEnumerable, IDisposable
        where T : unmanaged
    {
        public struct Enumerator : IEnumerator
        {
            private readonly IntPtr _head;
            private readonly int _capacity;
            private int _ofs;
            private int _sz;

            public Enumerator(IntPtr head, int len)
            {
                _sz = sizeof(T);
                _capacity = len * _sz;
                _head = head;
                _ofs = -_sz;
            }

            public T* Current => (T*)(_head + _ofs);
            object IEnumerator.Current => *Current;

            public bool MoveNext()
            {
                _ofs += _sz;
                if (_capacity <= _ofs)
                    return false;
                return true;
            }

            public void Reset()
            {
                _ofs = -_sz;
            }
        }

        public struct Indexer
        {
            private readonly IntPtr _head;
            private readonly int _szElement;

            public T* this[int index]
            {
                get
                {
                    var ofs = index * _szElement;
                    return (T*)(_head + ofs);
                }
            }

            public Indexer(IntPtr head, int szElement)
            {
                _head = head;
                _szElement = szElement;
            }
        }

        private readonly IntPtr _head;
        private readonly int _szElement;
        
        public int Capacity { get; }
        public int Length { get; private set; }
        public bool IsCreated => _head == IntPtr.Zero;

        public T* this[int index]
        {
            get
            {
                var ofs = index * _szElement;
                return (T*)(_head + ofs);
            }
            set
            {
                var ofs = index * _szElement;
                var ptr = (T*)(_head + ofs);
                *ptr = *value;
            }
        }
        
        public static umnArray<T> CreateNew<TAllocator>(ref TAllocator allocator, int maxLength)
            where TAllocator : unmanaged, IumnAllocator
        {
            var sz = sizeof(T);
            var chk = allocator.Calloc(maxLength * sz);
            return new umnArray<T>(chk);
        }

        public umnArray(umnChunk* chk)
        {
            _head = umnChunk.GetPtr(chk);
            _szElement = Marshal.SizeOf(typeof(T));
            Length = 0;

            Capacity = chk->length;
        }

        public void Clear()
        {
            Length = 0;
        }

        public void CClear()
        {
            UnmanagedHelper.Memset(_head.ToPointer(), 0, Capacity);
            Length = 0;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_head, Length);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Indexer GetIndexer()
        {
            return new Indexer(_head, _szElement);
        }

        public T* PushBack(T* e)
        {
            if (Capacity <= _szElement * Length)
                ThrowHelper.ThrowCapacityOverflow($"Capacity:{Capacity.ToString()}");

            var ofs = Length++ * _szElement;
            var ptr = (T*)(_head + ofs);
            *ptr = *e;
            return ptr;
        }

        public T* PushBack(T e)
        {
            if (Capacity <= _szElement * Length)
                ThrowHelper.ThrowCapacityOverflow($"Capacity:{Capacity.ToString()}");

            var ofs = Length++ * _szElement;
            var ptr = (T*)(_head + ofs);
            *ptr = e;
            return ptr;
        }

        public T* PopBack()
        {
            if (Length <= 0)
                ThrowHelper.ThrowCapacityOverflow($"Length:{Length.ToString()}");

            var ofs = --Length * _szElement;
            return (T*)(_head + ofs);
        }

        public T* GetLast()
        {
            if (Length <= 0)
                return null;

            var ofs = (Length - 1) * _szElement;
            return (T*)(_head + ofs);
        }

        public void Dispose()
        {
            var chk = umnChunk.GetChunk(_head);
            chk->Disposed = true;
        }
    }
}
