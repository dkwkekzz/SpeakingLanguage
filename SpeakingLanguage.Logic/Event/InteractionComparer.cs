using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal class InteractionComparer : IComparer<Interaction>
    {
        public int Compare(Interaction x, Interaction y)
        {
            var ret = x.subject.value.CompareTo(y.subject.value);
            if (ret != 0)
                return ret;
            return x.target.value.CompareTo(y.target.value);
        }
    }
}
