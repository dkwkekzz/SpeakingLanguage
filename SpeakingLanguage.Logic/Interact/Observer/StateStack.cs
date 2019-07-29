using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Interact
{
    internal class StateStack
    {
        private readonly StateValue[] _stack = new StateValue[Config.MAX_COUNT_STATE_BUFFER];
        private int head = 0;

        public StateValue Current => _stack[head];
        public StateValue Next
        {
            set
            {
                if (++head >= Config.MAX_COUNT_STATE_BUFFER)
                    head = 0;

                _stack[head] = value;
            }
        }
    }
}
