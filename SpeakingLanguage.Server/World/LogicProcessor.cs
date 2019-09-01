using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SpeakingLanguage.Server
{
    internal sealed class LogicProcessor : IDisposable
    {
        private CancellationTokenSource _cts;

        public void Dispose()
        {
            _cts.Cancel();
        }

        public LogicProcessor()
        {
        }

        public void RunAsync()
        {
            if (_cts == null)
                _cts = new CancellationTokenSource();

            var token = _cts.Token;
            Task.Factory.StartNew(() =>
            {
                var worldManager = WorldManager.Instance;
                var eventManager = Logic.EventManager.Instance;
                while (!token.IsCancellationRequested)
                {
                    eventManager.FrameEnter();

                    var userIter = worldManager.Agents.GetUserEnumerator();
                    while (userIter.MoveNext())
                    {
                        var user = userIter.Current;
                        user.FlushData();
                    }

                    var ret = eventManager.ExecuteFrame();
                    if (ret.Leg >= 0)
                        continue;

                    Thread.Sleep(ret.Leg);
                }
            }, token);
        }
    }
}
