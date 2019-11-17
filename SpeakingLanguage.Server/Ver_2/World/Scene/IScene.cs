using LiteNetLib;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal interface IScene : IDisposable
    {
        int Capacity { get; }
        int Count { get; }
        bool TryAddNotification(ISubscriber subscriber);
        bool CancelNotification(int id);
        void MoveTo(IScene dest);
        Dictionary<int, ISubscriber>.ValueCollection.Enumerator GetEnumerator();
    }
}
