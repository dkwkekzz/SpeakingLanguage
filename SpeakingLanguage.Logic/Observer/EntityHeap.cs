using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Logic
{
    class EntityHeap : IDisposable
    {
		struct Chunk
        {
            public IntPtr ptr;
            public int size;
        }

        private int _capacity;
        private IntPtr _root;
        private IntPtr _head;
        private IntPtr _tail;
        private List<Chunk> _garbage;

        public EntityHeap(int capacity)
        {
            _capacity = capacity;

            try
            {
                _root = _head = Marshal.AllocHGlobal(_capacity);
                _tail = IntPtr.Add(_head, _capacity);
            }
            catch (OutOfMemoryException oe)
            {
                _capacity >>= 1;
                _root = _head = Marshal.AllocHGlobal(_capacity);
            }
        }
		
        public IntPtr Allocate(int size)
        {
            var remained = _tail.ToInt64() - _head.ToInt64();
            if (remained < size)
            {
                remained = _compactClean();
                if (remained < size)
                {
                    var prevOffset = _head.ToInt64() - _root.ToInt64();
                    var nCapacity = _capacity * 2;
                    try
                    {
                        _head = Marshal.AllocHGlobal(nCapacity);
                        _tail = IntPtr.Add(_head, nCapacity);

                        unsafe
                        {
                            Buffer.MemoryCopy(_root.ToPointer(), _head.ToPointer(), prevOffset, prevOffset);
                        }

                        Marshal.FreeHGlobal(_root);

                        _root = _head;
                        _head += (int)prevOffset;
                        _capacity *= 2;
                    }
                    catch (OutOfMemoryException oe)
                    {   // 여기서 예외가 발생할 경우, 해당 컴포넌트를 수용할 방법이 없음.
                        throw oe;
                    }
                }
            }

            var ptr = _head;
            _head += size;
            return ptr;
        }

        public void Free(IntPtr ptr, int size)
        {
            if (null == _garbage)
                _garbage = new List<Chunk>();

            _garbage.Add(new Chunk { ptr = ptr, size = size });
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(_root);
        }

		public void Display()
        {
            Library.Tracer.Write($"=====[EntityHeap]=====");
            Library.Tracer.Write($"capacity: {_capacity}");
            Library.Tracer.Write($"per: {_head.ToInt64() - _root.ToInt64()} / {_tail.ToInt64() - _root.ToInt64()}");
            Library.Tracer.Write($"======================");
        }
		
        private int _compactClean()
        {
            if (null == _garbage || _garbage.Count == 0)
                return 0;
			
            _garbage.Sort((ck1, ck2) => { return ck1.ptr.ToInt64().CompareTo(ck2.ptr.ToInt64()); });
			
            IntPtr retHead = IntPtr.Zero; 
            IntPtr currentHead = _garbage[0].ptr;
            IntPtr currentTail = _garbage[0].ptr + _garbage[0].size;
			for (int i = 0; i != _garbage.Count - 1; i++)
            {
                var current = _garbage[i];
                if (currentHead == IntPtr.Zero)
                {
                    currentHead = current.ptr;
                    currentTail = currentHead + current.size;
                }
				
                if (currentTail == current.ptr)
                {
                    currentTail = current.ptr + current.size;
                    continue;
                }

                var next = _garbage[i + 1];
                var interval = next.ptr.ToInt64() - currentTail.ToInt64();
                unsafe
                {
                    Buffer.MemoryCopy(currentTail.ToPointer(), currentHead.ToPointer(), interval, interval);
                }

                retHead = currentHead + (int)interval;
                currentHead = IntPtr.Zero;
                currentTail = IntPtr.Zero;
            }

            {
                var endPtr = _head;
                var interval = endPtr.ToInt64() - currentTail.ToInt64();
                unsafe
                {
                    Buffer.MemoryCopy(currentTail.ToPointer(), currentHead.ToPointer(), interval, interval);
                }

                retHead = currentHead + (int)interval;
            }

            _head = retHead;
            _garbage.Clear();

            var remained = _tail.ToInt64() - _head.ToInt64();
            return (int)remained;
        }
    }
}
