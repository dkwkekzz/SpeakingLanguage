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

    public struct Controller : IEventData<Controller>
    {
        public ControlType type;
        public int objectHandleValue;
        public int key;
        public int value;

        public bool Equals(Controller other)
        {
            return type == other.type
                && objectHandleValue == other.objectHandleValue
                && key == other.key
                && value == other.value;
        }
    }
}
