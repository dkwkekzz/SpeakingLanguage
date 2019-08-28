using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal static class Director
    {
        public static unsafe void Execute(EventManager eventManager, ref Service service)
        {
            // select scene

            ref readonly var colObj = ref service.colObj;
            var objIter = colObj.GetEnumerator();
            while (objIter.MoveNext())
            {
                var objPtr = objIter.Current;
                var logicState = slObject.GetDefaultState(objPtr);

            }
        }
    }
}
