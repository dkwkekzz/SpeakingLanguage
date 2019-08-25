using LiteNetLib;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal interface IScene : IDisposable
    {
        IScene Left { get; }
        IScene Top { get; }
        IScene Right { get; }
        IScene Bottom { get; }
        IScene LeftTop { get; }
        IScene TopRight { get; }
        IScene RightBottom { get; }
        IScene BottomLeft { get; }

        int Capacity { get; }
        int Count { get; }
        bool TryInsert(Agent agent);
        bool Remove(int clientId);
        Dictionary<int, Agent>.ValueCollection.Enumerator GetEnumerator();
    }
}
