using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal unsafe class FrontStack
    {
        public struct Poster<TEvent> where TEvent : unmanaged
        {
            private readonly Library.umnDynamicArray* _stack;
            private readonly int _size;

            public Poster(Library.umnDynamicArray* stack)
            {
                _stack = stack;
                _size = sizeof(TEvent);

                var typeHandle = typeof(TEvent).TypeHandle.Value;
                _stack->PushChunk(typeHandle);
            }

            public void Push(TEvent e)
            {
                _stack->PushBack(&e, _size);
            }

            public void Push(void* p, int sz)
            {
                _stack->PushBack(p, sz);
            }
        }

        public struct Requester<TEvent> where TEvent : unmanaged
        {
            private readonly Library.umnDynamicArray* _stack;
            private readonly int _stSize;
            private bool _exit;

            public bool IsEmpty
            {
                get
                {
                    if (_exit)
                        return true;

                    if (_stack->IsHead)
                    {
                        _close();
                        return true;
                    }

                    return false;
                }
            }

            public Requester(Library.umnDynamicArray* stack)
            {
                _stack = stack;
                _stSize = sizeof(TEvent);
                _exit = false;

                var typeHandle = typeof(TEvent).TypeHandle.Value;
                if (typeHandle != _stack->CurrentTypeHandle)
                    Library.ThrowHelper.ThrowWrongState($"It is a different type than the marked type: marked:{_stack->CurrentTypeHandle} / target: {typeof(TEvent)}");
            }

            public TEvent* Pop()
            {
                if (_exit)
                    return null;

                var ret = _stack->PopBack(_stSize);
                if (null == ret)
                    _close();

                return (TEvent*)ret;
            }

            private void _close()
            {
                _exit = true;
                _stack->PopChunk();
            }
        }

        private readonly Library.umnDynamicArray _stack;

        public FrontStack(Library.umnHeap* allocator, int capacity)
        {
            _stack = Library.umnDynamicArray.AllocateNew(allocator, capacity);
        }

        public Poster<TEvent> GetPoster<TEvent>() where TEvent : unmanaged
        {
            fixed (Library.umnDynamicArray* pst = &_stack)
                return new Poster<TEvent>(pst);
        }

        public Requester<TEvent> GetRequester<TEvent>() where TEvent : unmanaged
        {
            fixed (Library.umnDynamicArray* pst = &_stack)
                return new Requester<TEvent>(pst);
        }

    }
}
