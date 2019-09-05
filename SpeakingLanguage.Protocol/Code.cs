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
            SelectSubject,
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
            NullReferenceCollider,
            NullReferenceSubsrciber,
            NullReferenceSubjectHandle,
            DuplicateAgent,
            OverflowSubscribe,
            FailToAuthentication,
            IllegalityDataForSelectSubject,
            IllegalityDataForInteraction,
            FailToSerialize,
            FailToDeserialize,
            FailToInteraction,
            __MAX__
        }
    }
}
