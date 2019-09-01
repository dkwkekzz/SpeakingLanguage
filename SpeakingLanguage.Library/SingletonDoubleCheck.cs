using System;
using System.Threading;

namespace SpeakingLanguage.Library
{
    public class SingletonDoubleCheck<T> 
        where T : class, new()
    {
        private static readonly object _lock = new object();
        private static T _value = null;
        
        public static T Instance
        {
            get
            {
                if (_value != null) return _value;

                Monitor.Enter(_lock);

                if (_value == null)
                {
                    T temp = new T();
                    Volatile.Write(ref _value, temp);
                }

                Monitor.Exit(_lock);

                return _value;
            }
        }
        public static bool IsCreated => null != _value;

        protected SingletonDoubleCheck()
        {
        }
    }
}
