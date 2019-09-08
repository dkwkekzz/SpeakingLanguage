using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal interface ISerializable
    {
        void DeserializeInfo(ref Library.Reader reader);
        void SerializeInfo(ref Library.Writer writer);
    }
}
