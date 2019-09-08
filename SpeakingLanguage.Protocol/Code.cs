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
            ConstructIdentifier,
            Terminate,
            SerializeUser,
            DeserializeUser,
            Keyboard,
            Touch,
            SelectSubject,
            CreateSubject,
            ResponseSelectSubject,
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
            IllegalityDataForSelectSubject,
            IllegalityDataForInteraction,
            FailToAuthentication,
            FailToSerialize,
            FailToDeserialize,
            FailToInteraction,
            __MAX__
        }
    }
}
