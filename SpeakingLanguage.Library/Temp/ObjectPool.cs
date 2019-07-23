using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Library.Temp
{
    // SafetyWorkspace : 스레드에 안전한 오브젝트를 반환해주는 스레드를 제공한다.
    // 리드온리제공
    // 추가,  개인스레드별공간에 추가
    // 실제동기화시점은 중요하지 않다... 
    // 즉시 동기화되어 얻어와야하는 객체가 있다면, 그러한 인터페이스를 별도로 제공한다.
    // 매니저스레드를 최우선순위로 즉시 구동시키고, 끝나자마자 해당 함수를 호출하는 방식으로 진행한다.
    
    public class ObjectPool<TKey, TValue>
        where TValue : struct
    {
        private readonly string _name;
        private int _capacity;
        private BitArray _sync;
        private TValue[] _buffer;
        private IDictionary<TKey, int> _keyLookup;
        private int _current;

        public ObjectPool(string name, int capacity)
        {
            _name = name;
            _capacity = capacity;
            _sync = new BitArray(capacity);
            _keyLookup = new Dictionary<TKey, int>(capacity);

            _buffer = new TValue[capacity];
            for (int i = 0; i != capacity; i++)
                _buffer[i] = new TValue();
        }

        public ObjectPool(string name, int capacity, IEqualityComparer<TKey> comparer)
        {
            _name = name;
            _capacity = capacity;
            _sync = new BitArray(capacity);
            _keyLookup = new Dictionary<TKey, int>(capacity, comparer);

            _buffer = new TValue[capacity];
            for (int i = 0; i != capacity; i++)
                _buffer[i] = new TValue();
        }

        public void Enlarge()
        {
            try
            {
                _capacity *= 2;
                var syncArr = new Byte[_capacity];
                _sync.CopyTo(syncArr, 0);
                var sync = new BitArray(syncArr);
                var buffer = new TValue[_capacity];
                var keyLookup = new Dictionary<TKey, int>(_capacity);
                _buffer.CopyTo(buffer, 0);
                _keyLookup.CopyTo(keyLookup);

                _sync = sync;
                _buffer = buffer;
                _keyLookup = keyLookup;
            }
            catch { }
        }

        public void Allocate(TKey key)
        {
            int objectKey;
            if (!_keyLookup.TryGetValue(key, out objectKey))
            {
                objectKey = _supplyObjectKey();
                _keyLookup.Add(key, objectKey);
            }

            _sync.Set(objectKey, true);
        }

        public void Free(TKey key)
        {
            int deleted;
            if (!_keyLookup.TryGetValue(key, out deleted))
                return;

            _sync.Set(deleted, false);
            _keyLookup.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int finded;
            if (!_keyLookup.TryGetValue(key, out finded))
            {
                value = default(TValue);
                return false;
            }

            if (_sync.Get(finded))
            {
                value = default(TValue);
                return false;
            }

            value = _buffer[finded];
            return true;
        }

        private int _supplyObjectKey()
        {
            int count = 0;
            int temp;
            do
            {
                if (count++ > _sync.Count)
                    throw new ArgumentOutOfRangeException($"buffer overflow at objectPool: {_name}");
                temp = _current++;
            } while (_sync.Get(temp) == false);

            _current %= _capacity;
            return temp;
        }
    }
}
