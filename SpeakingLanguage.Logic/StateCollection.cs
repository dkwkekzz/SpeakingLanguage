using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal struct StateHandle
    {
        public int key;
        public Type value;
    }

    internal class StateCollection
    {
        private static Dictionary<Type, StateHandle> _dicHandle;

        static StateCollection()
        {
            Build();
        }

        public static void Build()
        {
            if (_dicHandle != null)
                _dicHandle.Clear();

            int indexer = 0;

            var dicHandle = new Dictionary<Type, StateHandle>();
            var types = Library.AssemblyHelper.CollectType((Type t) => { return t.GetInterface("SpeakingLanguage.Logic.IState") != null; });
            foreach (var type in types)
            {
                dicHandle.Add(type, new StateHandle { key = indexer++, value = type });
            }

            _dicHandle = dicHandle;
        }

        public static StateHandle GetStateHandle(Type t)
        {
            return _dicHandle[t];
        }
    }
}
