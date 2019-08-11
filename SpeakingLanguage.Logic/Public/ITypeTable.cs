using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public interface ITypeTable
    {
        int this[Type t] { get; }
    }
}
