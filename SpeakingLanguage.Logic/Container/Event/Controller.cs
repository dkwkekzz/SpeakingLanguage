using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public enum ControlType
    {
        None = 0,
        Keyboard,
        Touch,
    }

    public struct Controller : IEquatable<Controller>
    {
        public ControlType type;
        public int subjectHandleValue;
        public int key;
        public int value;

        public bool Equals(Controller other)
        {
            return type == other.type
                && subjectHandleValue == other.subjectHandleValue
                && key == other.key
                && value == other.value;
        }
    }
}
