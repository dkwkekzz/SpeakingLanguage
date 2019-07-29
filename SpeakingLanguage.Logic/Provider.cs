using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public sealed class Provider : IDisposable
    {
        private readonly Terminal _terminal;
        private readonly World.Service _world;
        private readonly Interact.Service _interact;

        public Provider(StartInfo info)
        {
            _terminal = new Terminal(ref info);
            _world = new World.Service(ref info);
            _interact = new Interact.Service(ref info);
        }

        public void Dispose()
        {
            _terminal.Dispose();
            _world.Dispose();
            _interact.Dispose();
        }

        public void ExecuteFrame(float delta)
        {
            _terminal.BeginFrame(delta);

            _world.OnEvent(_terminal);
            _interact.OnEvent(_terminal);
        }

        public unsafe void Inject<TEvent>(TEvent e) where TEvent : unmanaged
        {
            var poster = _terminal.GetBackPoster<TEvent>();
            poster.Push(&e, sizeof(TEvent));
        }
    }
}
