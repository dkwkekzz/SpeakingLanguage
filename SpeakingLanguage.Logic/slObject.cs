using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe struct slObject : IDisposable
    {
        public struct Key
        {
            public int handle;

            public static implicit operator Key(int h)
            {
                return new Key { handle = h };
            }

            public static bool operator ==(Key key, int h)
            {
                return key.handle == h;
            }

            public static bool operator !=(Key key, int h)
            {
                return key.handle != h;
            }
        }

        public struct Enumerator
        {
            private Library.umnChunk* root;
            private Library.umnChunk* cur;

            public Library.umnChunk* Current => cur;

            public Enumerator(Library.umnChunk* chk)
            {
                root = chk;
                cur = null;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (null == cur)
                    cur = root;
                else
                    cur = cur->next;
                return cur != null;
            }

            public void Reset()
            {
                cur = null;
            }
        }

        private Library.umnHeap _block;
        private Library.umnChunk* _root;

        public int Handle { get; private set; }
        public int BlockSize => _block.Capacity;

        public static unsafe slObject* Create<TAllocator>(TAllocator* allocator, int handle, int szBlock) 
            where TAllocator : unmanaged, Library.IumnAllocator
        {
            var objChk = allocator->Alloc(sizeof(slObject));
            var objPtr = (slObject*)Library.umnChunk.GetPtr(objChk);
            objPtr->_block = new Library.umnHeap(allocator->Alloc(szBlock));
            objPtr->_root = null;
            objPtr->Handle = handle;
            return objPtr;
        }

        public slObject(int handle, Library.umnChunk* chk)
        {
            _block = new Library.umnHeap(chk);
            _root = null;
            Handle = handle;
        }

        public void Take(int handle)
        {
            _block.Reset();
            _root = null;
            Handle = handle;
        }

        public void Release()
        {
            Handle = 0;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_root);
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
            var chk = _block.Calloc(sz);
            if (null == chk)
                Library.ThrowHelper.ThrowOutOfMemory($"in observer inner heap\ncapacity:{_block.Capacity.ToString()}");

            var typeHandle = type.TypeHandle.Value;
            chk->typeHandle = typeHandle;
            
            if (_root == null)
            {
                _root = chk;
                return Library.umnChunk.GetPtr(chk);
            }
            
            Library.umnChunk* prev = null;
            Library.umnChunk* cur = _root;
            while (null != cur)
            {
                if ((long)cur->typeHandle > (long)typeHandle)
                    break;

                prev = cur;
                cur = cur->next;
            }

            if (null != prev)
                prev->next = chk;
            chk->next = cur;

            return Library.umnChunk.GetPtr(chk);
        }

        public bool RemoveEntity<TEntity>()
        {
            var typeHandle = typeof(TEntity).TypeHandle.Value;
            var chk = _getEntityChunk(typeHandle, _root);
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
            var chk = _getEntityChunk(typeHandle, _root);
            if (chk == null)
                return null;

            return (TEntity*)chk->Ptr;
        }

        public TEntity* GetEntity<TEntity>(Library.umnChunk* startChk) where TEntity : unmanaged
        {
            var typeHandle = typeof(TEntity).TypeHandle.Value;
            var chk = _getEntityChunk(typeHandle, startChk);
            if (chk == null)
                return null;

            return (TEntity*)chk->Ptr;
        }

        private Library.umnChunk* _getEntityChunk(IntPtr typeHandle, Library.umnChunk* head) 
        {
            while (null != head)
            {
                if (head->typeHandle == typeHandle)
                    return head;

                head = head->next;
            }
            
            return null;
        }

        public void Dispose()
        {
            _block.Dispose();
        }
    }
}
