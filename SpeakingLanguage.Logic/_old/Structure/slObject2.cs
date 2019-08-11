using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe struct slObject2 : IDisposable
    {
        public struct Key
        {
            public int handle;

            public static implicit operator Key(int h)
            {
                return new Key { handle = h };
            }
        }
        
        private Library.umnHeap _heap;
        private Library.umnChunk* _head;

        public int Handle { get; private set; }
        public Library.umnChunk* EntityHead => _head;

        public slObject2(int handle, Library.umnChunk* chk)
        {
            Handle = handle;

            _heap = new Library.umnHeap(chk);
            _head = null;
        }

        public void Take(int handle, Library.umnChunk* chk)
        {
            Handle = handle;

            if (!_heap.IsCreated)
                _heap = new Library.umnHeap(chk);
            else
                _heap.Reset();

            _head = null;
        }

        public void Release()
        {
            Handle = 0;
        }
        
        public TEntity* AddEntity<TEntity>() where TEntity : unmanaged
        {
            int sz = sizeof(TEntity);
            var type = typeof(TEntity);
            var ptr = _addEntityImpl(sz, type);
            return (TEntity*)ptr;
        }

        public TEntity* AddEntity<TEntity>(TEntity e) where TEntity : unmanaged
        {
            int sz = sizeof(TEntity);
            var type = typeof(TEntity);
            var ptr = _addEntityImpl(sz, type);
            Buffer.MemoryCopy(&e, ptr.ToPointer(), sz, sz);
            return (TEntity*)ptr;
        }

        private IntPtr _addEntityImpl(int sz, Type type)
        {
            var chk = _heap.Calloc(sz);
            if (null == chk)
                Library.ThrowHelper.ThrowOutOfMemory($"in observer inner heap\ncapacity:{_heap.Capacity.ToString()}");

            var typeHandle = type.TypeHandle.Value;
            chk->typeHandle = typeHandle;

            if (_head == null)
            {
                _head = chk;
                return IntPtr.Zero;
            }

            var phead = _head;
            if (phead->typeHandle.ToInt64() > typeHandle.ToInt64())
            {
                while (phead != null && phead->typeHandle.ToInt64() > typeHandle.ToInt64())
                {
                    if (phead->typeHandle == typeHandle)
                        break;
                    phead = phead->prev;
                }

                var prev = phead;
                var cur = chk;
                var next = phead->next;
                prev->next = cur;
                cur->prev = prev;
                cur->next = next;
                next->prev = cur;
                next->prev = cur;
            }
            else if (phead->typeHandle.ToInt64() < typeHandle.ToInt64())
            {
                while (phead != null && phead->typeHandle.ToInt64() < typeHandle.ToInt64())
                {
                    if (phead->typeHandle == typeHandle)
                        break;
                    phead = phead->next;
                }

                var prev = phead->prev;
                var cur = chk;
                var next = phead;
                prev->next = cur;
                cur->prev = prev;
                cur->next = next;
                next->prev = cur;
                next->prev = cur;
            }

            return chk->Ptr;
        }

        public bool RemoveEntity<TEntity>()
        {
            var typeHandle = typeof(TEntity).TypeHandle.Value;
            var chk = _getEntityChunk(typeHandle);
            if (chk == null)
                return false;

            chk->Disposed = true;
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

            return (TEntity*)chk->Ptr;
        }

        public TEntity* NextEntity<TEntity>() where TEntity : unmanaged
        {
            if (_head == null)
                return null;

            var prevTypeHandle = _head->typeHandle;

            _head = _head->next;
            if (_head->typeHandle != prevTypeHandle)
                return null;

            return (TEntity*)_head->Ptr;
        }

        private Library.umnChunk* _getEntityChunk(IntPtr typeHandle) 
        {
            var phead = _head;
            if (phead->typeHandle.ToInt64() < typeHandle.ToInt64())
            {
                while (phead != null)
                {
                    if (phead->typeHandle == typeHandle)
                        return _head = phead;
                    phead = phead->next;
                }
            }
            else
            {
                while (phead != null)
                {
                    if (null == phead->prev || phead->prev->typeHandle.ToInt64() < typeHandle.ToInt64())
                        return _head = phead;
                    phead = phead->prev;
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
