using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal static class Factory
    {
        public static unsafe void Execute(EventManager eventManager, ref Service service)
        {
            var hLifeCycle = StateCollection.GetStateHandle(typeof(LifeCycle));
            var hSpawner = StateCollection.GetStateHandle(typeof(Spawner));

            ref readonly var colObj = ref service.colObj;
            var objIter = colObj.GetEnumerator();
            while (objIter.MoveNext())
            {
                var objPtr = objIter.Current;
                var stateIter = objPtr->GetEnumerator();
                if (!stateIter.MoveNext()) continue;

                var chk = stateIter.Current;
                if (chk->typeHandle == hLifeCycle.key)
                {
                    var lcPtr = Library.umnChunk.GetPtr<LifeCycle>(chk);
                    if (lcPtr->value == 0)
                        colObj.Destroy(objPtr);

                    if (!stateIter.MoveNext()) continue;
                    chk = stateIter.Current;
                }

                if (chk->typeHandle == hSpawner.key)
                {
                    var spPtr = Library.umnChunk.GetPtr<Spawner>(chk);
                    colObj.Create(spPtr->value);
                }
            }
        }
    }
}
