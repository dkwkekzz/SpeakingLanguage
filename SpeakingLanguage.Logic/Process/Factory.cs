using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Process
{
    internal static class Factory
    {
        public static unsafe void Execute(EventManager eventManager, ref Service service)
        {
            ref readonly var colObj = ref service.colObj;
            var objIter = colObj.GetEnumerator();
            while (objIter.MoveNext())
            {
                var objPtr = objIter.Current;
                var logicState = slObjectHelper.GetDefaultState(objPtr);

                var life = logicState->lifeCycle;
                if (life.value == 0)
                {
                    colObj.Destroy(objPtr);
                }

                var spawner = logicState->spawner;
                if (spawner.value > 0)
                {
                    colObj.Create(spawner.value);
                }
            }
        }
    }
}
