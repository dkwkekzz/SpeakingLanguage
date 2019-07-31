using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe interface IPublicContext
    {
        float Delta { get; }
        int Frame { get; }
        float FrameTick { get; }

        WorldActor This { get; }
        WorldActor Other { get; }
        ICommander<BinaryCommand> Commander { get; }
    }
}
