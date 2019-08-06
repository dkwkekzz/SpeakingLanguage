using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpeakingLanguage.Table
{
    [DebuggerDisplay("{Value}")]
    public struct String128
    {
        public String64 str01;
        public String64 str02;

        public string Value => StringHelper.ToManagedString(this);
    }
}
