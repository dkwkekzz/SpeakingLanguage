using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal struct sCellComparer : IComparer<sCell.Key>
    {
        public int Compare(sCell.Key x, sCell.Key y)
        {
            return x.handle.CompareTo(y.handle);
        }
    }
}
