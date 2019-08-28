using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal interface ISubscriber
    {
        int Id { get; }
        Logic.slObjectHandle SubjectHandle { get; }
        void Push(NetDataWriter writer);
    }
}
