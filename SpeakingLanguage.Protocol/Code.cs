using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Protocol
{
    public static class Code
    {
        public enum Packet
        {
            None = 0,
            Authentication,
            Terminate,
            Keyboard,
            Touch,
            SelectScene,
            SubscribeScene,
            UnsubscribeScene,
            Interaction,
            __MAX__
        }

        public enum Error
        {
            None = 0,
            NullReferenceScene,
            NullReferenceAgent,
            AlreadyExistAgent,
            __MAX__
        }
    }
}
