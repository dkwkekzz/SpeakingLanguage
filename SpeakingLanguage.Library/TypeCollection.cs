using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public struct TypeHandle
    {
        public int key;
        public Type value;
    }

    public class TypeCollection
    {
        private static readonly IReadOnlyDictionary<Type, TypeHandle> _dicTypeHandle;

        public static readonly TypeHandle disposed;
        public static readonly TypeHandle @null;
        public static readonly TypeHandle sbtPairNode;

        static TypeCollection()
        {
            int indexer = 1;

            var dicTypeHandle = new Dictionary<Type, TypeHandle>();
            var types = Library.AssemblyHelper.CollectType(null);
            foreach (var type in types)
            {
                dicTypeHandle.Add(type, new TypeHandle { key = indexer++, value = type });
            }

            _dicTypeHandle = dicTypeHandle;

            disposed = new TypeHandle { key = -1 };
            @null = new TypeHandle { key = 0 };
            sbtPairNode = _dicTypeHandle[typeof(sbtPairNode)];
        }

        public static TypeHandle GetTypeHandle(Type t)
        {
            return _dicTypeHandle[t];
        }
    }

    public static class umnTypeHandle
    {
        public static readonly IntPtr sbtPairNode = typeof(sbtPairNode).TypeHandle.Value;

    }
}
