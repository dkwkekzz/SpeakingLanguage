using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SpeakingLanguage.Server
{
    internal class LogicProcessor : IDisposable
    {
        private Logic.EventManager _eventManager;

        public void Dispose()
        {
        }

        public LogicProcessor()
        {
            _eventManager = Logic.EventManager.Locator;
        }

        public void Update(ref Logic.Service service)
        {
            _eventManager.ExecuteFrame(ref service);
        }
    }
}
