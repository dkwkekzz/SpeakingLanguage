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
            Subscribe,
            Control,
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
            InvaildController,
            NoExistFile,
            FailToDeserialize,

            __MAX__
        }
    }
}
