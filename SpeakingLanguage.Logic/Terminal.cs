using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal sealed class Terminal : IDisposable
    {
        public struct Poster<TEvent> where TEvent : unmanaged
        {
            private readonly UnmanagedStack _stack;
            
            public Poster(UnmanagedStack stack)
            {
                _stack = stack;
                _stack.EntryPoint(typeof(TEvent));
            }
            
            public unsafe void Push(TEvent e)
            {
                //if (_stack.CurrentType != typeof(TEvent))
                //    throw new InvalidOperationException($"It is a different type than the selected type: selected:{_stack.CurrentType} / current: {typeof(TEvent)}");

                _stack.Push(&e);
            }

            public unsafe void Push(void* p, int sz)
            {
                _stack.Push(p, sz);
            }
        }

        public struct Requester<TEvent> where TEvent : unmanaged
        {
            private readonly UnmanagedStack _stack;
            private bool _exit;

            public bool IsEmpty
            {
                get
                {
                    if (_stack.IsMarked)
                    {
                        close();
                        return true;
                    }

                    return false;
                }
            }

            public Requester(UnmanagedStack stack)
            {
                _stack = stack;
                _exit = false;

                if (typeof(TEvent) != _stack.CurrentType)
                    throw new InvalidOperationException($"It is a different type than the marked type: marked:{_stack.CurrentType} / current: {typeof(TEvent)}");
            }
            
            public unsafe TEvent* Pop()
            {
                if (_exit)
                    return null;

                return (TEvent*)_stack.Pop();
            }

            private void close()
            {
                if (!_exit)
                    _stack.ExitPoint();

                _exit = true;
            }
        }

        private readonly UnmanagedStack _frontStack;
        private readonly UnmanagedStack _backStack;
        
        public FrameTicker Ticker { get; }
        public float Delta { get; private set; }

        public Terminal(ref StartInfo info)
        {
            _frontStack = new UnmanagedStack(info.max_byte_terminal);
            _backStack = new UnmanagedStack(info.max_byte_terminal);
            
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
            return new Poster<TEvent>(_frontStack);
        }

        public Requester<TEvent> GetRequester<TEvent>() where TEvent : unmanaged
        {
            return new Requester<TEvent>(_frontStack);
        }

        public Poster<TEvent> GetBackPoster<TEvent>() where TEvent : unmanaged
        {
            return new Poster<TEvent>(_backStack);
        }

        public Requester<TEvent> GetBackRequester<TEvent>() where TEvent : unmanaged
        {
            return new Requester<TEvent>(_backStack);
        }
    }
}
