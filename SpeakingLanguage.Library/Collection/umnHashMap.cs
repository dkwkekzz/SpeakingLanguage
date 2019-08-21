using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    internal unsafe struct HashEntry
    {
        public int hashCode;        
        public int next;            
        public void* key;           
        public void* value;         
    }

    public unsafe struct umnHashMap<TComparer, TKey, TValue>
        where TComparer : unmanaged, IumnEqualityComparer<TKey>
        where TKey : unmanaged
        where TValue : unmanaged
    {
        private TComparer comparer;
        private umnArray<int> buckets;
        private umnArray<HashEntry> entries;
        private int maxLength;
        private int count;
        private int freeList;
        private int freeCount;

        public umnHashMap(umnChunk* bucketChk, umnChunk* entryChk, int size)
        {
            comparer = new TComparer();
            buckets = new umnArray<int>(bucketChk);
            for (int i = 0; i < buckets.Length; i++) *(buckets[i]) = -1;
            entries = new umnArray<HashEntry>(entryChk);
            maxLength = size;
            count = 0;
            freeCount = 0;
            freeList = -1;
        }

        public static umnHashMap<TComparer, TKey, TValue> CreateNew<TAllocator>(ref TAllocator allocator, int capacity) 
            where TAllocator : unmanaged, IumnAllocator
        {
            int size = HashHelper.GetPrime(capacity);
            return new umnHashMap<TComparer, TKey, TValue>(
                allocator.Calloc(size * sizeof(int)), 
                allocator.Calloc(size * sizeof(HashEntry)), 
                size);
        }

        public TValue* this[TKey key]
        {
            get
            {
                int i = _findEntry(&key);
                if (i >= 0) return (TValue*)entries[i]->value;
                ThrowHelper.ThrowKeyNotFound();
                return null;
            }
            set
            {
                _insert(&key, value, false);
            }
        }

        public void Add(TKey* key, TValue* value)
        {
            _insert(key, value, true);
        }

        public void Clear()
        {
            if (count > 0)
            {
                for (int i = 0; i < buckets.Length; i++) *buckets[i] = -1;
                entries.Clear();
                freeList = -1;
                count = 0;
                freeCount = 0;
            }
        }

        public bool ContainsKey(TKey key)
        {
            return _findEntry(&key) >= 0;
        }

        public bool TryGetValue(TKey* key, out TValue* value)
        {
            int i = _findEntry(key);
            if (i >= 0)
            {
                value = (TValue*)entries[i]->value;
                return true;
            }
            value = null;
            return false;
        }
        
        public bool Remove(TKey* key)
        {
            HashEntry* entry = null;

            int hashCode = comparer.GetHashCode(key) & 0x7FFFFFFF;
            int bucket = hashCode % maxLength;
            int last = -1;
            for (int i = *buckets[bucket]; i >= 0; last = i, i = entry->next)
            {
                entry = entries[i];
                if (entry->hashCode == hashCode && comparer.Equals((TKey*)entry->key, key))
                {
                    if (last < 0)
                    {
                        *buckets[bucket] = entry->next;
                    }
                    else
                    {
                        entries[last]->next = entry->next;
                    }
                    entry->hashCode = -1;
                    entry->next = freeList;
                    entry->key = null;
                    entry->value = null;
                    freeList = i;
                    freeCount++;
                    return true;
                }
            }

            return false;
        }

        private int _findEntry(TKey* key)
        {
            HashEntry* entry = null;

            int hashCode = comparer.GetHashCode(key) & 0x7FFFFFFF;
            for (int i = *buckets[hashCode % maxLength]; i >= 0; i = entry->next)
            {
                entry = entries[i];
                if (entry->hashCode == hashCode && comparer.Equals((TKey*)entry->key, key)) return i;
            }

            return -1;
        }

        private void _insert(TKey* key, TValue* value, bool add)
        {
            int hashCode = comparer.GetHashCode(key) & 0x7FFFFFFF;
            int targetBucket = hashCode % maxLength;

            HashEntry* entry = null;

            for (int i = *buckets[targetBucket]; i >= 0; i = entry->next)
            {
                entry = entries[i];
                if (entry->hashCode == hashCode && comparer.Equals((TKey*)entry->key, key))
                {
                    if (add)
                    {
                        ThrowHelper.ThrowWrongArgument("Argument_AddingDuplicate.");
                    }
                    entry->value = value;
                    return;
                }
            }

            int index;
            if (freeCount > 0)
            {
                index = freeList;
                entry = entries[index];
                freeList = entry->next;
                freeCount--;
            }
            else
            {
                if (count == maxLength)
                {
                    _resize(HashHelper.ExpandPrime(maxLength));
                    targetBucket = hashCode % maxLength;
                }
                index = count;
                entry = entries[index];
                count++;
            }

            entry->hashCode = hashCode;
            entry->next = *(buckets[targetBucket]);
            entry->key = key;
            entry->value = value;
        }

        private void _resize(int newSize)
        {
            throw new NotImplementedException();
            //int[] newBuckets = new int[newSize];
            //for (int i = 0; i < newBuckets.Length; i++) newBuckets[i] = -1;
            //Entry[] newEntries = new Entry[newSize];
            //Array.Copy(entries, 0, newEntries, 0, count);
            //
            //for (int i = 0; i < count; i++)
            //{
            //    if (newEntries[i].hashCode >= 0)
            //    {
            //        int bucket = newEntries[i].hashCode % newSize;
            //        newEntries[i].next = newBuckets[bucket];
            //        newBuckets[bucket] = i;
            //    }
            //}
            //buckets = newBuckets;
            //entries = newEntries;
        }
    }
}
