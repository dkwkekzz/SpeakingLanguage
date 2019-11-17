using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpeakingLanguage.Library
{
    [DebuggerDisplay("{Value}")]
    public struct String32
    {
        public String16 str01;
        public String16 str02;

        public string Value => StringHelper.ToManagedString(this);
    }
}
