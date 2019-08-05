using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public sealed class Provider : IDisposable
    {
        private readonly Locator _terminal;

        public Provider(StartInfo info)
        {
            _terminal = new Locator(ref info);
        }

        public void Dispose()
        {
            _terminal.Dispose();
        }

        public unsafe void ExecuteFrame()
        {
            _terminal.FrameManager.Begin();

            var obDic = _terminal.ObserverDictionary;
            var obIter = obDic.GetEnumerator();
            while (obIter.MoveNext())
            {
                var ob = obIter.Current;
                var obs = ob->GetEntity<Entity.Observer>();
                if (null == obs)
                    return;

                // 대상을 여기서 찾아주어야 한다.
                var targetOb = obDic.Find(obs->targetHandle);
                if (null == targetOb)
                    return;

                // execute
            }
        }
    }
}
