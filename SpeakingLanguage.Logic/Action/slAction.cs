using System;
using System.Collections.Generic;
using System.Reflection;

namespace SpeakingLanguage.Logic
{
    internal struct slAction<TKey> : IComparable<slAction<TKey>> where TKey : IComparable<TKey>
    {   // 타입을 구분하여 이벤트의 처리의 우선순위를 높여야한다.
        public delegate void InteractDelegate(ref ActionContext ctx);
        public InteractDelegate Invoke { get; }
        public TKey Key { get; }
        
        public slAction(MethodInfo mth, TKey key)
        {
            Invoke = (InteractDelegate)mth.CreateDelegate(typeof(InteractDelegate));
            Key = key;
        }

        public int CompareTo(slAction<TKey> other)
        {
            return Key.CompareTo(other.Key);
        }
    }
}
