using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public class InteractionAttribute : Attribute
    {
        public Type SrcType { get; }
        public Define.Relation Relation { get; }

        public InteractionAttribute(Type src, Define.Relation relation)
        {
            SrcType = src;
            Relation = relation;
        }
    }
}
