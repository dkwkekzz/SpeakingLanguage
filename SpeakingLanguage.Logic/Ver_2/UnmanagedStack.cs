using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Logic
{
    internal unsafe class UnmanagedStack : IDisposable
    {
        private struct Mark
        {
            public Type type;
            public IntPtr ofs;
            public int size;
        }

        private int _capacity;
        private IntPtr _root;
        private IntPtr _head;
        private IntPtr _tail;
        private Stack<Mark> _stMarks = new Stack<Mark>();
        private Mark _current;

        public bool IsEmpty => _root == _head;
        public Type CurrentType => _current.type;
        public bool IsMarked => _current.ofs == _head;

        public UnmanagedStack(int capacity)
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

        public void EntryPoint(Type stType)
        {
            _current = new Mark { type = stType, ofs = _head, size = Marshal.SizeOf(stType) };
            _stMarks.Push(_current);
        }

        public void ExitPoint()
        {
            if (!IsMarked)
                _head = _current.ofs;

            _stMarks.Pop();

            if (_stMarks.Count == 0)
                _current = default(Mark);
            else
                _current = _stMarks.Peek();
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(_root);
        }

        public void Push(void* stPtr)
        {
            if (_stMarks.Count == 0)
                Library.ThrowHelper.ThrowWrongState("You must take new type before push.");

            var size = _current.size;
            Buffer.MemoryCopy(stPtr, _head.ToPointer(), size, size);
            _head += size;
        }

        public void Push(void* stPtr, int size)
        {
            if (_stMarks.Count == 0)
                Library.ThrowHelper.ThrowWrongState("You must take new type before push.");
            
            Buffer.MemoryCopy(stPtr, _head.ToPointer(), size, size);
            _head += size;
        }

        public void* Pop()
        {
            if (_stMarks.Count == 0)
                Library.ThrowHelper.ThrowWrongState("You must take new type before Pop.");

            if (IsMarked)
                Library.ThrowHelper.ThrowWrongState("You can't pop after marked.");

            var size = _current.size;
            _head -= size;
            return _head.ToPointer();
        }
    }
}
