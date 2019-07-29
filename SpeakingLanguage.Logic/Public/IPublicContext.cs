using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public interface IPublicContext
    {
        float Delta { get; }
        int Frame { get; }
        float FrameTick { get; }
        IEntityManager This { get; }
        ITypeTable TypeTable { get; }
        ICommander<BinaryCommand> Commander { get; }
    }
}
