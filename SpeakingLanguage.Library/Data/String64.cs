using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpeakingLanguage.Library
{
    [DebuggerDisplay("{Value}")]
    public struct String64
    {
        public String32 str01;
        public String32 str02;

        public string Value => StringHelper.ToManagedString(this);
    }
}
