using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal enum InteractDirection
    {
        None,
        Forward = 1000,
        Backward,
        BidirectForwardFirst,
        BidirectBackwardFirst,
    }

    internal struct Interaction : IEventData<Interaction>
    {
        public slObjectHandle subject;
        public slObjectHandle target;
        public InteractDirection dir;

        public bool Equals(Interaction other)
        {
            return subject.value == other.subject.value && target.value == other.target.value;
        }
    }
}
