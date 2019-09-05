using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal struct Interaction : IEquatable<Interaction>
    {
        public slObjectHandle subject;
        public slObjectHandle target;

        public bool Equals(Interaction other)
        {
            return subject.value == other.subject.value && target.value == other.target.value;
        }
    }
}
