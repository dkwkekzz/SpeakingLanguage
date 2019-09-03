using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal struct StateSyncPair : IComparable<StateSyncPair>
    {
        public StateSync subject;
        public StateSync target;

        public int CompareTo(StateSyncPair other)
        {
            var ret = subject.CompareTo(other.subject);
            if (ret != 0)
                return ret;

            return target.CompareTo(other.target);
        }

        public static StateSyncPair Create<TEnumerator>(TEnumerator iter1, TEnumerator iter2)
            where TEnumerator : IEnumerator<int>
        {
            var key = new StateSyncPair();
            key.subject = StateSync.Create(ref iter1);
            key.target = StateSync.Create(ref iter2);

            return key;
        }
    }
}
