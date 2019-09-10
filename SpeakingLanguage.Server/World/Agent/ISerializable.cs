using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal interface ISerializable
    {
        void OnDeserialize(ref Library.Reader reader);
        void OnSerialize(ref Library.Writer writer);
    }
}
