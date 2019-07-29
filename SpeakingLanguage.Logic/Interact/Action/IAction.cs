using System;
using System.Collections.Generic;
using System.Reflection;

namespace SpeakingLanguage.Logic.Interact
{
    internal interface IAction
    {
        void Take(MethodInfo mth);
        void Invoke(ref CallContext ctx);
    }
}
