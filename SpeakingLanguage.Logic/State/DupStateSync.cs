using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal struct DupStateSync : IComparable<DupStateSync>
    {
        public StateSync subject;
        public StateSync target;

        public int CompareTo(DupStateSync other)
        {
            var ret = subject.CompareTo(other.subject);
            if (ret != 0)
                return ret;

            return target.CompareTo(other.target);
        }

        public static DupStateSync Create<TEnumerator>(TEnumerator iter1, TEnumerator iter2)
            where TEnumerator : IEnumerator<int>
        {
            var key = new DupStateSync();
            key.subject = StateSync.Create(ref iter1);
            key.target = StateSync.Create(ref iter2);

            return key;
        }
    }
}
