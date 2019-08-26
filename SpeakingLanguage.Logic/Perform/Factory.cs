﻿using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
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

                var lcPtr = slObject.GetLifeCycle(objPtr);
                if (lcPtr->value == 0)
                {
                    colObj.Destroy(objPtr);
                }

                var spPtr = slObject.GetSpawner(objPtr);
                if (spPtr->value > 0)
                {
                    colObj.Create(spPtr->value);
                }
            }
        }
    }
}
