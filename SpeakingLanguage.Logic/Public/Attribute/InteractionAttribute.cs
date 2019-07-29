using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public class InteractionAttribute : Attribute
    {
        public Type SrcType { get; }

        public InteractionAttribute(Type src)
        {
            SrcType = src;
        }
    }
}
