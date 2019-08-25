using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public struct ChangeView : IEventData<ChangeView>
    {
        public int left;
        public int right;
        public int top;
        public int bottom;

        public bool Equals(ChangeView other)
        {
            return left == other.left
                && right == other.right
                && top == other.top
                && bottom == other.bottom;
        }
    }
}
