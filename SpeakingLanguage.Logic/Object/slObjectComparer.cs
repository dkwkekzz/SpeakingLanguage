using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal unsafe struct slObjectComparer : Library.IumnComparer<Logic.slObjectHandle>
    {
        public int Compare(Logic.slObjectHandle* x, Logic.slObjectHandle* y)
        {
            var xh = x->value;
            var yh = y->value;
            if (xh == yh)
                return 0;
            return xh < yh ? -1 : 1;
        }
    }
}
