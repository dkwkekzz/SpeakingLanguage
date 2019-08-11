using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public class ConditionAttribute : Attribute
    {
        public Type[] SrcType { get; }

        public ConditionAttribute(params Type[] src)
        {
            SrcType = src;
        }
    }

    public class TargetAttribute : Attribute
    {
        public Type[] SrcType { get; }

        public TargetAttribute(params Type[] src)
        {
            SrcType = src;
        }
    }

    public class InteractionAttribute : Attribute
    {
        public int ConditionCount { get; }

        public InteractionAttribute(int conditionCount)
        {
            ConditionCount = conditionCount;
        }
    }
}
