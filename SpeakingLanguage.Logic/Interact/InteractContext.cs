using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public class InteractContext : IPublicContext
    {
        public float Delta { get; set; }
        public int Frame { get; set; }
        public float FrameTick { get; set; }
        public IEntityManager This { get; set; }
        public ITypeTable TypeTable { get; set; }
        public ICommander<BinaryCommand> Commander { get; set; }
    }
}
