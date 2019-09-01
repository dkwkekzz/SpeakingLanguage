using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal interface ISubscriber : IAgent
    {
        NetDataWriter DataWriter { get; }
    }
}
