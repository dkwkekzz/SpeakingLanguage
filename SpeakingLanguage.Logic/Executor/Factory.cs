using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Executor
{
    internal static class Factory
    {
        public static unsafe void Execute(Locator terminal)
        {
            var obDic = terminal.ObserverDictionary;
            
            var obIter = obDic.GetEnumerator();
            while (obIter.MoveNext())
            {
                var ob = obIter.Current;
                var spawner = ob->GetEntity<Entity.Spawner>();
                if (null != spawner)
                {
                    int count = spawner->count;
                    while (count-- > 0)
                    {
                        var node = obDic.CreateObserver(spawner->blockSize);
                        obDic.Add(node);
                    }
                }

                var life = ob->GetEntity<Entity.LifeCycle>();
                if (null != life)
                {
                    if (life->value <= 0)
                    {
                        obDic.Remove(ob);
                    }
                }
            }
        }
    }
}
