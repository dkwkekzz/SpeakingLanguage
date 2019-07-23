using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public class BoxedWriter
    {
        public Writer Value;
        public BoxedWriter(int capacity = 1 << 6)
        {
            this.Value = new Writer(capacity);
        }
    }
}
