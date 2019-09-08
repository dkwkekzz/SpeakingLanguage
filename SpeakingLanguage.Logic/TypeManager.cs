using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Logic
{
    internal struct TypeHandle
    {
        public int key;
        public Type value;
        public int size;
    }

    internal sealed class TypeManager : Library.SingletonLazy<TypeManager>
    {
        private IReadOnlyDictionary<Type, TypeHandle> _dicHandle;

        public TypeHandle SHNull { get; } = new TypeHandle { key = 0, value = null, size = 0 };
        public TypeHandle SHDefaultState { get; } = new TypeHandle { key = 1, value = typeof(Default), size = Marshal.SizeOf<Default>() };
        public TypeHandle SHControlState { get; } = new TypeHandle { key = 2, value = typeof(Control), size = Marshal.SizeOf<Control>() };

        public TypeManager()
        {
            Build();
        }

        public void Build()
        {
            var dicHandle = new Dictionary<Type, TypeHandle>()
            {
                { typeof(Library.Null), SHNull },
                { SHDefaultState.value, SHDefaultState },
                { SHControlState.value, SHControlState },
            };

            int indexer = dicHandle.Count;
            var types = Library.AssemblyHelper.CollectType((Type t) => { return t.GetInterface("SpeakingLanguage.Logic.IState") != null; });
            foreach (var type in types)
            {
                if (dicHandle.ContainsKey(type))
                    continue;

                dicHandle.Add(type, new TypeHandle { key = indexer++, value = type, size = Marshal.SizeOf(type) });
            }

            _dicHandle = dicHandle;
        }

        public static TypeHandle GetStateHandle(Type t)
        {
            return Instance._dicHandle[t];
        }
    }
}
