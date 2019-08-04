using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Executor
{
    internal static class Transform
    {
        public static unsafe void Execute(Locator terminal)
        {
            var poster = terminal.FrontStack.GetPoster<Event.Transform>();

            var obDic = terminal.ObserverDictionary;
            var obIter = obDic.GetEnumerator();
            while (obIter.MoveNext())
            {
                var ob = obIter.Current;
                var phy = ob->GetEntity<Entity.Location>();
                if (null != phy)
                {
                    //if (phy->changed)
                    //{
                    //    poster.Push(new Event.Transform { actor = ob, x = phy->x, y = phy->y, layer = phy->layer });
                    //    phy->changed = false;
                    //}
                }
            }
        }
    }
}
