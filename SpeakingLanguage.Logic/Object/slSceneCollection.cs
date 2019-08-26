using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal struct slScene
    {

    }

    internal struct slSceneCollection
    {
        private readonly Library.umnMarshal _umnAllocator;
        private Library.umnHeap _scenes;
        private Library.umnHashMap<slSceneEqualityComparer, slSceneHandle, slScene> _lookup;

        public slSceneCollection(int defaultSceneCount)
        {
            _umnAllocator = new Library.umnMarshal();
            _lookup = Library.umnHashMap<slSceneEqualityComparer, slSceneHandle, slScene>.CreateNew(ref _umnAllocator, defaultSceneCount);
        }

        public 
    }
}
