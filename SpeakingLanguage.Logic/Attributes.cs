using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public sealed class SubjectConditionAttribute : Attribute
    {
        public Type[] ParamTypes { get; }

        public SubjectConditionAttribute(params Type[] types)
        {
            this.ParamTypes = types;
        }
    }

    public sealed class TargetConditionAttribute : Attribute
    {
        public Type[] ParamTypes { get; }

        public TargetConditionAttribute(params Type[] types)
        {
            this.ParamTypes = types;
        }
    }
}
