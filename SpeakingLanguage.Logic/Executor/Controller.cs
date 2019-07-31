using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal static class Controller
    {
        public unsafe static void Execute(Locator terminal)
        {
            var obDic = terminal.ObserverDictionary;
            var obIter = obDic.GetEnumerator();
            while (obIter.MoveNext())
            {
                var ob = obIter.Current;
                var ctrl = ob->GetEntity<Property.Controller>();
                if (null != ctrl)
                {
                    switch (ctrl->operation)
                    {
                        case 0:
                            ob->SetState(ctrl->type, ctrl->value);
                            break;
                        case 1:
                            ob->IncreaseState(ctrl->type, ctrl->value);
                            break;
                    }
                }
            }
        }
    }
}
