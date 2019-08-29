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

    internal class TypeManager
    {
        private static readonly TypeManager _instance = new TypeManager();

        private static Dictionary<Type, TypeHandle> _dicHandle;

        public static TypeHandle SHChunk { get; } = new TypeHandle { key = -1, value = typeof(Library.umnChunk), size = Marshal.SizeOf<Library.umnChunk>() };
        public static TypeHandle SHObject { get; } = new TypeHandle { key = -2, value = typeof(slObject), size = Marshal.SizeOf<slObject>() };

        public static TypeHandle SHDefaultState { get; } = new TypeHandle { key = 0, value = typeof(Default), size = Marshal.SizeOf<Default>() };
        public static TypeHandle SHControlState { get; } = new TypeHandle { key = 1, value = typeof(Control), size = Marshal.SizeOf<Control>() };

        static TypeManager()
        {
            Build();
        }

        public static void Build()
        {
            if (_dicHandle != null)
                _dicHandle.Clear();
            
            var dicHandle = new Dictionary<Type, TypeHandle>()
            {
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
            return _dicHandle[t];
        }
    }
}
