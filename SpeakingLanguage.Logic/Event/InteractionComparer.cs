﻿using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal class InteractionComparer : IComparer<Interaction>
    {
        public int Compare(Interaction x, Interaction y)
        {
            var ret = x.lhs.value.CompareTo(y.lhs.value);
            if (ret != 0)
                return ret;
            return x.rhs.value.CompareTo(y.rhs.value);
        }
    }
}