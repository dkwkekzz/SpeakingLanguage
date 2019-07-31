using System;
using System.Collections.Generic;
using System.Reflection;

namespace SpeakingLanguage.Logic
{
    internal interface IAction
    {
        void Take(MethodInfo mth);
        void Invoke(CallContext ctx);
    }
}
