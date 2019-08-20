using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe struct slObjectComparer : Library.IumnComparer<Logic.slObjectHandle>
    {
        public int Compare(Logic.slObjectHandle* x, Logic.slObjectHandle* y)
        {
            var xh = x->handle;
            var yh = y->handle;
            if (xh == yh)
                return 0;
            return xh < yh ? -1 : 1;
        }
    }
}
