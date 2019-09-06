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
    }

    public struct EventResult
    {
        public EventError error;
        public int intVal;
        public bool Success => error == EventError.None;

        public EventResult(EventError err = EventError.None, int iVal = 0)
        {
            error = err;
            intVal = iVal;
        }
    }
}
