using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal unsafe sealed class Terminal : IDisposable
    {
        public struct Poster<TEvent> where TEvent : unmanaged
        {
            private readonly Library.umnDynamicArray* _stack;
            private readonly int _size;
            
            public Poster(Library.umnDynamicArray* stack)
            {
                _stack = stack;
                _size = sizeof(TEvent);

                int typeIdx = 0;
                _stack->PushChunk(typeIdx);
            }
            
            public void Push(TEvent e)
            {
                //if (_stack.CurrentType != typeof(TEvent))
                //    throw new InvalidOperationException($"It is a different type than the selected type: selected:{_stack.CurrentType} / current: {typeof(TEvent)}");
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
            private readonly int _size;
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
                _size = sizeof(TEvent);
                _exit = false;

                int typeIdx = 0;
                if (typeIdx != _stack->CurrentTypeIdx)
                    throw new InvalidOperationException($"It is a different type than the marked type: marked:{_stack->CurrentTypeIdx} / target: {typeof(TEvent)}");
            }
            
            public TEvent* Pop()
            {
                if (_exit)
                    return null;

                var ret = _stack->PopBack(_size);
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

        //private readonly UnmanagedStack _frontStack;
        //private readonly UnmanagedStack _backStack;
        private readonly Library.umnMarshal _marshal;
        private readonly Library.umnDynamicArray _frontStack;
        private readonly Library.umnDynamicArray _backStack;

        public FrameTicker Ticker { get; }
        public float Delta { get; private set; }

        public Terminal(ref StartInfo info)
        {
            _marshal = new Library.umnMarshal();

            fixed (Library.umnMarshal* pMs = &_marshal)
            {
                _frontStack = Library.umnDynamicArray.AllocateNew(pMs, info.max_byte_terminal);
                _backStack = Library.umnDynamicArray.AllocateNew(pMs, info.max_byte_terminal);
            }
            //_frontStack = new UnmanagedStack(info.max_byte_terminal);
            //_backStack = new UnmanagedStack(info.max_byte_terminal);

            Ticker = new FrameTicker(info.startFrame);
        }

        public void Dispose()
        {
            _frontStack.Dispose();
            _backStack.Dispose();
        }

        public void BeginFrame(float delta)
        {
            Delta = delta;
            Ticker.Begin();
        }

        public Poster<TEvent> GetPoster<TEvent>() where TEvent : unmanaged
        {
            fixed (Library.umnDynamicArray* pStack = &_frontStack)
            {
                return new Poster<TEvent>(pStack);
            }
        }

        public Requester<TEvent> GetRequester<TEvent>() where TEvent : unmanaged
        {
            fixed (Library.umnDynamicArray* pStack = &_frontStack)
            {
                return new Requester<TEvent>(pStack);
            }
        }

        public Poster<TEvent> GetBackPoster<TEvent>() where TEvent : unmanaged
        {
            fixed (Library.umnDynamicArray* pStack = &_backStack)
            {
                return new Poster<TEvent>(pStack);
            }
        }

        public Requester<TEvent> GetBackRequester<TEvent>() where TEvent : unmanaged
        {
            fixed (Library.umnDynamicArray* pStack = &_backStack)
            {
                return new Requester<TEvent>(pStack);
            }
        }
    }
}
