using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SpeakingLanguage.Library
{
    public sealed class ListPool<TValue> : KeyObjectPool<int, List<TValue>>
    {
        public ListPool() : base(capacity => { return new List<TValue>(capacity); }, list => { list.Clear(); })
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<TValue> GetList(int capacity)
        {
            return base.GetObject(1 << (Math.Log2ge(capacity)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PutList(List<TValue> list)
        {
            base.PutObject(list.Capacity, list);
        }
    }
}
