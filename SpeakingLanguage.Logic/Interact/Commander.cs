using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Interact
{
    internal class Commander : ICommander<BinaryCommand>, IEnumerable<BinaryCommand>
    {
        public struct CleanEnumerator : IEnumerator<BinaryCommand>
        {
            private Dictionary<Type, BinaryCommand>.Enumerator orgItr;
            Library.ObjectPool<BinaryCommand> pool;

            public CleanEnumerator(Commander cmd)
            {
                orgItr = cmd._dicCmds.GetEnumerator();
                pool = cmd._poolCmd;
            }

            public BinaryCommand Current => orgItr.Current.Value;
            object IEnumerator.Current { get { return this.Current; } }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                var prev = Current;
                if (null  != prev)
                    pool.PutObject(prev);

                return orgItr.MoveNext();
            }
            
            public void Reset()
            {
            }
        }
        
        private readonly Dictionary<Type, BinaryCommand> _dicCmds;
        private readonly Library.ObjectPool<BinaryCommand> _poolCmd;
        private readonly int _capacity;

        public Commander(int cmdTypeCount)
        {
            _dicCmds = new Dictionary<Type, BinaryCommand>(cmdTypeCount);
            _poolCmd = new Library.ObjectPool<BinaryCommand>(cmdTypeCount);
            _capacity = cmdTypeCount;
        }
        
        public BinaryCommand GetCommand(Type type)
        {
            if (_dicCmds.Count >= _capacity)
                Library.ThrowHelper.ThrowCapacityOverflow("in _sbtCmds");

            if (_poolCmd.Capacity == 0)
                Library.ThrowHelper.ThrowCapacityOverflow("in _poolCmd");

            if (!_dicCmds.TryGetValue(type, out BinaryCommand cmd))
            {
                cmd = _poolCmd.GetObject();
                _dicCmds.Add(type, cmd);
            }

            cmd.Take(type);
            return cmd;
        }
        
        public CleanEnumerator GetEnumerator()
        {
            return new CleanEnumerator(this);
        }

        IEnumerator<BinaryCommand> IEnumerable<BinaryCommand>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
