using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Logic.Interact
{
    unsafe sealed class _EntityManager : IEntityManager, IEnumerable<KeyValuePair<Type, IntPtr>>, IDisposable
    {
        private readonly UnmanagedHeap _heap;
        private readonly Dictionary<Type, IntPtr> _dicType2Ptr;
        private readonly Observer _parent;

        public int Handle => _parent.Handle;
        public IState<StateValue> State => _parent.State;

        public _EntityManager(Observer parent)
        {
            _heap = new UnmanagedHeap(Config.MAX_BYTE_ENTITYMANAGER_HEAP);
            _dicType2Ptr = new Dictionary<Type, IntPtr>(Config.MAX_COUNT_ENTITY_TYPE);
            _parent = parent;
        }

        public Dictionary<Type, IntPtr>.Enumerator GetEnumerator()
        {
            return _dicType2Ptr.GetEnumerator();
        }

        IEnumerator<KeyValuePair<Type, IntPtr>> IEnumerable<KeyValuePair<Type, IntPtr>>.GetEnumerator()
        {
            return _dicType2Ptr.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddEntity<TEntity>() where TEntity : unmanaged
        {
            var type = typeof(TEntity);
            var sz = Marshal.SizeOf(type);
            var ptr = _heap.Alloc(sz);

            _dicType2Ptr.Add(type, ptr);
        }

        public void RemoveEntity<TEntity>(TEntity e) where TEntity : unmanaged
        {
            var type = typeof(TEntity);
            var sz = Marshal.SizeOf(type);
            var ptr = _dicType2Ptr[type];

            Library.UnmanagedHelper.Memset(ptr.ToPointer(), 0, sz);
            _dicType2Ptr.Remove(type);
        }

        public void SetEntity<TEntity>(TEntity e) where TEntity : unmanaged
        {
            var type = typeof(TEntity);
            var sz = Marshal.SizeOf(type);
            var ptr = _dicType2Ptr[type];

            Buffer.MemoryCopy(&e, ptr.ToPointer(), sz, sz);
        }

        public TEntity* GetEntity<TEntity>() where TEntity : unmanaged
        {
            var type = typeof(TEntity);
            if (!_dicType2Ptr.TryGetValue(type, out IntPtr ptr))
                return null;

            return (TEntity*)ptr;
        }

        public void Reset()
        {
            _heap.Reset();
        }

        public void Dispose()
        {
            _heap.Dispose();
        }
    }

    internal unsafe struct sEntity
    {

    }

    public unsafe struct sEntityManager
    {

    }

    internal unsafe struct sStateMap
    {

    }

    internal unsafe struct sObserver : IDisposable
    {
        private const int LEN_STATE_STACK = 16;

        private readonly Library.umnHeap* _rootHeap;
        private readonly Library.umnHeap _heap;
        private readonly Library.umnArray<Library.umnHeap, StateValue> _stateMap;
        private readonly int _blockSize;
        private Library.umnChunk* _head;

        public int Handle { get; private set; }

        public sObserver(Library.umnHeap* rootHeap, int blockSize)
        {
            _rootHeap = rootHeap;
            _heap = new Library.umnHeap(rootHeap->Alloc(blockSize));
            _stateMap = Library.umnArray<Library.umnHeap, StateValue>.Allocate_umnArray(_rootHeap, (int)Define.Controller.__MAX__ * LEN_STATE_STACK);
            _blockSize = blockSize;
            _head = null;
            Handle = 0;
        }

        public void Take(int handle)
        {
            Handle = handle;
        }

        public void Release()
        {
            Handle = 0;
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

        public bool RemoveEntity(int typeIdx)
        {
            var chk = _getEntityChunk(typeIdx);
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

        public TEntity* GetEntity<TEntity>(int typeIdx) where TEntity : unmanaged
        {
            var chk = _getEntityChunk(typeIdx);
            if (chk == null)
                return null;

            return (TEntity*)chk->ptr;
        }

        private Library.umnChunk* _getEntityChunk(int typeIdx) 
        {
            var fh = _head;
            var bh = _head;
            while (fh != null || bh != null)
            {
                if (fh != null)
                {
                    if (fh->typeIdx == typeIdx)
                        return _head = fh;
                    fh = fh->next;
                }

                if (bh != null)
                {
                    if (bh->typeIdx == typeIdx)
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

    internal class Observer : IDisposable
    {
        private readonly _EntityManager _entityMnr;

        public IEntityManager EntityManager => _entityMnr;
        public StateMap State { get; }
        public int Handle { get; private set; }

        public Observer()
        {
            _entityMnr = new _EntityManager(this);
            State = new StateMap();
        }

        public void Take(int handle)
        {
            Handle = handle;
        }

        public void Release()
        {
            _entityMnr.Reset();
            Handle = 0;
        }

        public void Dispose()
        {
            _entityMnr.Dispose();
        }
    }
}
