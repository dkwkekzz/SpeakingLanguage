using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal struct Collider
    {
        public Logic.Position position;
    }

    internal class ColliderCollection
    {
        private class _colliderEqualityComparer : IEqualityComparer<ColliderHandle>
        {
            public bool Equals(ColliderHandle x, ColliderHandle y)
            {
                return x.objHandleValue == y.objHandleValue;
            }

            public int GetHashCode(ColliderHandle obj)
            {
                return obj.objHandleValue;
            }
        }

        private Dictionary<ColliderHandle, Collider> _dicColliders;

        public ColliderCollection() : this(0)
        {
        }

        public ColliderCollection(int capacity)
        {
            _dicColliders = new Dictionary<ColliderHandle, Collider>(capacity, new _colliderEqualityComparer());
        }

        public void Update(Logic.slObjectHandle objHandle, Logic.Position pos)
        {
            _dicColliders[new ColliderHandle(objHandle)] = new Collider
            {
                position = pos,
            };
        }
    }
}
