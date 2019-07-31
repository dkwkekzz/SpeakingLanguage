using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public sealed class Provider : IDisposable
    {
        private readonly Locator _terminal;
        private readonly World.Service _world;
        private readonly Interact.Service _interact;

        public Provider(StartInfo info)
        {
            _terminal = new Locator(ref info);
            _world = new World.Service(ref info);
            _interact = new Interact.Service(ref info);
        }

        public void Dispose()
        {
            _terminal.Dispose();
        }

        public void ExecuteFrame()
        {
            _terminal.FrameManager.Begin();

            _world.OnEvent(_terminal);
            _interact.OnEvent(_terminal);
        }

        public unsafe void InjectToInteract(Event.Controller ctrl)
        {
            _interact.InjectController(&ctrl);
        }
    }
}
