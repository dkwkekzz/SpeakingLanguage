using LiteNetLib;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal interface IScene : IDisposable, IEnumerable<ISubscriber>
    {
        int Capacity { get; }
        int Count { get; }
        bool TryAddSubscribe(ISubscriber subscriber);
        bool CancelSubscribe(int id);
        void MoveTo(IScene dest);
        Dictionary<int, ISubscriber>.ValueCollection.Enumerator GetEnumerator();
    }
}
