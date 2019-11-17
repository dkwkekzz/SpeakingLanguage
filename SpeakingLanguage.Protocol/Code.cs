using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Protocol
{
    public static class Code
    {
        public enum Packet
        {
            None = 0,
            Error,
            Success,
            Connection,
            Disconnection,
            Authentication,
            Synchronization,
            Control,
            Select,
            Subscribe,
            Interaction,
            __MAX__
        }

        public enum Error
        {
            None = 0,
            NullReferenceAgent,
            NullReferenceScene,
            NullReferenceObject,
            NullReferenceArchtype,
            DuplicateAuthentication,
            InvaildAuthentication,
            NoExistFile,
            FailToDeserialize,

            __MAX__
        }
    }
}
