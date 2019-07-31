using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe class InteractContext : IPublicContext
    {
        public float Delta { get; set; }
        public int Frame { get; set; }
        public float FrameTick { get; set; }

        public WorldActor This { get; set; }
        public WorldActor Other { get; set; }
        public ICommander<BinaryCommand> Commander { get; set; }
    }
}
