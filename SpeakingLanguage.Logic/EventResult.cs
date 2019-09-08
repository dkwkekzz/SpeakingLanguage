using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public enum EventError
    {
        None = 0,
        FailToReadLength,
        FailToReadHandle,
        NullReferenceObject,
        NullReferenceControlState,
        SelfInteraction,
        OverflowObjectCapacity,
    }

    public struct EventResult
    {
        public EventError error;
        public bool Success => error == EventError.None;

        public EventResult(EventError err = EventError.None)
        {
            error = err;
        }
    }

    public struct EventResult<TResult> where TResult : struct
    {
        public EventError error;
        public TResult result;
        public bool Success => error == EventError.None;

        public EventResult(EventError err = EventError.None, TResult ret = default)
        {
            error = err;
            result = ret;
        }
    }
}
