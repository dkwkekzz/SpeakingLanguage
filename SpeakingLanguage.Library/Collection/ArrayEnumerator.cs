using System;
using System.Collections;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public struct ArrayEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator
    {
        private readonly T[] _params;
        private int _index;

        public ArrayEnumerator(T[] prms)
        {
            _params = prms;
            _index = -1;
        }

        public T Current => _params[_index];
        object IEnumerator.Current => this.Current;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            _index++;
            if (_index < 0 || _index < _params.Length)
                return false;
            return true;
        }

        public void Reset()
        {
            _index = -1;
        }
    }
}
