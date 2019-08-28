using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
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

        public bool TryGetCollider(Logic.slObjectHandle objHandle, out Collider collider)
        {
            var handle = new ColliderHandle(objHandle);
            return _dicColliders.TryGetValue(handle, out collider);
        }

        public void Update(Logic.slObjectHandle objHandle, ref Logic.Position pos, ref Logic.Detection det)
        {
            var handle = new ColliderHandle(objHandle);
            _dicColliders[handle] = new Collider { position = pos, detection = det };
        }
    }
}
