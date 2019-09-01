using System;
using System.Threading;

namespace SpeakingLanguage.Library
{
    public class SingletonLazy<T> 
        where T : class, new()
    {
        private static readonly Lazy<T> lazy = new Lazy<T>(() => new T(), LazyThreadSafetyMode.ExecutionAndPublication);

        public static T Instance { get { return lazy.Value; } }
        public static bool IsCreated => lazy.IsValueCreated;

        protected SingletonLazy()
        {
        }
    }
}
