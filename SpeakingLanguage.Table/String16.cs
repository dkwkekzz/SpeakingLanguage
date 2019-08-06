using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SpeakingLanguage.Table
{
    [DebuggerDisplay("{Value}")]
    public struct String16
    {
        public char c00;
        public char c01;
        public char c02;
        public char c03;
        public char c04;
        public char c05;
        public char c06;
        public char c07;
        public char c08;
        public char c09;
        public char c10;
        public char c11;
        public char c12;
        public char c13;
        public char c14;
        public char c15;

        public string Value => StringHelper.ToManagedString(this);
    }
}
