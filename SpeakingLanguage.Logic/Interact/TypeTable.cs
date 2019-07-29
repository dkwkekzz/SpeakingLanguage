using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Interact
{
    internal sealed class TypeTable : ITypeTable
    {
        private readonly IReadOnlyDictionary<Type, int> _dicStructs;

        public TypeTable()
        {
            _dicStructs = _collectStructs();
        }

        public int this[Type t] => _dicStructs[t];

        private Dictionary<Type, int> _collectStructs()
        {
            var dic = new Dictionary<Type, int>();

            var types = Library.AssemblyHelper.CollectType(null);
            foreach (var type in types)
            {
                if (type.IsClass)
                    continue;

                dic.Add(type, dic.Count);
            }

            return dic;
        }
    }
}
