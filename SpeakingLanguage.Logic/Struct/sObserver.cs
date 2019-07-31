using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal unsafe struct sObserver : IDisposable
    {
        public struct Key
        {
            public int handle;

            public static implicit operator Key(int h)
            {
                return new Key { handle = h };
            }
        }
        
        private Library.umnHeap* _rootHeap;
        private Library.umnHeap _heap;
        private int _blockSize;
        private Library.umnNativeArray _stateMap;
        private Library.umnChunk* _head;

        public int Handle { get; private set; }
        public Library.umnChunk* EntityHead => _head;

        public void Take(int handle, Library.umnHeap* rootHeap, int blockSize)
        {
            Handle = handle;

            if (null == _rootHeap)
            {
                _rootHeap = rootHeap;
                _heap = new Library.umnHeap(rootHeap->Alloc(blockSize));
                _blockSize = blockSize;
                _stateMap = Library.umnNativeArray.AllocateNew<Library.umnHeap, StateValue>(_rootHeap, (int)Define.Controller.__MAX__);
            }
            else
            {
                _heap.Reset();
                _stateMap.Reset();
            }

            _head = null;
        }

        public void Release()
        {
            Handle = 0;
        }

        public StateValue* GetState(int type)
        {
            return (StateValue*)_stateMap[type];
        }

        public StateValue* GetState(Define.Controller type)
        {
            return (StateValue*)_stateMap[(int)type];
        }

        public void SetState(int type, int value)
        {
            var val = new StateValue { value = value };
            _stateMap[type] = &val;
        }

        public void IncreaseState(int type, int value)
        {
            var state = GetState(type);
            var val = new StateValue { value = state->value + value };
            _stateMap[type] = &val;
        }

        public TEntity* AddEntity<TEntity>() where TEntity : unmanaged
        {
            int sz = sizeof(TEntity);
            var chk = _heap.Calloc(sz);
            if (null == chk)
            {
                var extChk = _rootHeap->Alloc(_blockSize);
                if (null == extChk)
                    Library.ThrowHelper.ThrowOutOfMemory($"in interact heap\ncapacity:{_rootHeap->TotalCapacity.ToString()}");

                _heap.Extend(extChk);
                chk = _heap.Calloc(sizeof(TEntity));
                if (chk == null)
                    Library.ThrowHelper.ThrowOutOfMemory($"in observer inner heap\ncapacity:{_heap.TotalCapacity.ToString()}");
            }

            if (_head == null)
                _head = chk;

            return (TEntity*)chk->ptr;
        }

        public void AddEntity<TEntity>(TEntity e) where TEntity : unmanaged
        {
            int sz = sizeof(TEntity);
            var chk = _heap.Calloc(sz);
            if (null == chk)
            {
                var extChk = _rootHeap->Alloc(_blockSize);
                if (null == extChk)
                    Library.ThrowHelper.ThrowOutOfMemory($"in interact heap\ncapacity:{_rootHeap->TotalCapacity.ToString()}");

                _heap.Extend(extChk);
                chk = _heap.Calloc(sizeof(TEntity));
                if (chk == null)
                    Library.ThrowHelper.ThrowOutOfMemory($"in observer inner heap\ncapacity:{_heap.TotalCapacity.ToString()}");
            }

            Buffer.MemoryCopy(&e, chk->ptr.ToPointer(), sz, sz);
        }

        public bool RemoveEntity<TEntity>()
        {
            var typeHandle = typeof(TEntity).TypeHandle.Value;
            var chk = _getEntityChunk(typeHandle);
            if (chk == null)
                return false;

            chk->dispose = true;
            return true;
        }

        public void SetEntity<TEntity>(TEntity* src, TEntity value) where TEntity : unmanaged
        {
            var sz = sizeof(TEntity);
            Buffer.MemoryCopy(&value, src, sz, sz);
        }

        public TEntity* GetEntity<TEntity>() where TEntity : unmanaged
        {
            var typeHandle = typeof(TEntity).TypeHandle.Value;
            var chk = _getEntityChunk(typeHandle);
            if (chk == null)
                return null;

            return (TEntity*)chk->ptr;
        }

        private Library.umnChunk* _getEntityChunk(IntPtr typeHandle) 
        {
            var fh = _head;
            var bh = _head;
            while (fh != null || bh != null)
            {
                if (fh != null)
                {
                    if (fh->typeHandle == typeHandle)
                        return _head = fh;
                    fh = fh->next;
                }

                if (bh != null)
                {
                    if (bh->typeHandle == typeHandle)
                        return _head = bh;
                    bh = bh->prev;
                }
            }
            
            return null;
        }

        public void Dispose()
        {
            _heap.Dispose();
        }
    }
}
