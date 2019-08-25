using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Protocol
{
    public static class Code
    {
        public enum Packet
        {
            None = 0,
            Keyboard,
            Touch,
            SelectScene,
            Interaction,
            __MAX__
        }

        public enum Error
        {
            None = 0,
            NullReferenceScene,
            __MAX__
        }
    }
}
