using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal struct sCellNodeComparer : IComparer<sCellNode.Key>
    {
        public int Compare(sCellNode.Key x, sCellNode.Key y)
        {
            return x.obHandle.CompareTo(y.obHandle);
        }
    }
}
