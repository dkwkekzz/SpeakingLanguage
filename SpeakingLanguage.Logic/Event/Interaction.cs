using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal struct Interaction : IEventData<Interaction>
    {
        public slObjectHandle lhs;
        public slObjectHandle rhs;

        public bool Equals(Interaction other)
        {
            return lhs.value == other.lhs.value && rhs.value == other.rhs.value;
        }
    }
}
