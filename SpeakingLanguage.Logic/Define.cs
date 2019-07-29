using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public class Define
    {
        public enum Object
        {
            None = 0,
            Observer,
            __MAX__
        }

        public enum Relation
        {
            None = 0,
            Self,
            Simple,
            Complex,
            __MAX__
        }

        public enum Controller
        {
            None = 0,
            Key_Left,
            Key_Right,
            Key_Up,
            Key_Down,
            __MAX__
        }

        public enum Layer
        {
            None = 0,
            World = 1,
        }
    }
}
