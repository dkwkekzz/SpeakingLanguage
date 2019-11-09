using System;
using System.Collections.Generic;
using System.Reflection;

namespace SpeakingLanguage.Logic
{
    internal enum ActionType
    {
        None = 0,
        Self,
        Interact,
    }

    internal unsafe interface IAction
    {
        ActionType Type { get; }
        void Initialize(MethodInfo mth, Type[] condTypes, Type[] targetTypes);
        void Invoke(StreamingContext* ctx);
    }
}
