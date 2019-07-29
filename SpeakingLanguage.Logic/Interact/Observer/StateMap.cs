using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Interact
{
    internal sealed class StateMap : IState<StateValue>
    {
        private List<StateStack> _lstVal = new List<StateStack>((int)Define.Controller.__MAX__);

        public StateValue Get(int type)
        {
            return _lstVal[type].Current;
        }

        public StateValue Get(Define.Controller type)
        {
            return _lstVal[(int)type].Current;
        }

        public void Set(int type, int value)
        {
            _lstVal[type].Next = new StateValue { value = value };
        }

        public void Increase(int type, int value)
        {
            var state = _lstVal[(int)type].Current;
            _lstVal[type].Next = new StateValue { value = state.value + value };
        }
    }
}
