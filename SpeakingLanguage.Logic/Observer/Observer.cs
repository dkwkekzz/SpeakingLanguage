using System;
using System.Collections.Generic;
using System.Threading;

namespace SpeakingLanguage.Logic
{
    class Observer : IDisposable
    {
        private readonly EntityHeap _heap;

        public int X { get; }
        public int Y { get; }
        public int Redius { get; }
        
        public Observer(int capacity)
        {
            _heap = new EntityHeap(capacity);
        }
        
        public void Dispose()
        {
            _heap.Dispose();
        }

    }
}
