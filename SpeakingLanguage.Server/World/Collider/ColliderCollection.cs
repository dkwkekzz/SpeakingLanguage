using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal class ColliderCollection
    {
        private Dictionary<int, Collider> _dicColliders;

        public ColliderCollection() : this(0)
        {
        }

        public ColliderCollection(int capacity)
        {
            _dicColliders = new Dictionary<int, Collider>(capacity);
        }

        public bool TryGetCollider(int objHandleValue, out Collider collider)
        {
            return _dicColliders.TryGetValue(objHandleValue, out collider);
        }

        public void Update(int objHandleValue)
        {
            _dicColliders[objHandleValue] = new Collider { isCreated = false };
        }

        public void Update(int objHandleValue, ref Logic.Position pos, ref Logic.Detection det)
        {
            _dicColliders[objHandleValue] = new Collider { isCreated = true, position = pos, detection = det };
        }
    }
}
