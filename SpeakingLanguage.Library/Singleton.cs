using System;

namespace SpeakingLanguage.Library
{
    public sealed class Singleton<T> 
        where T : class, new()
    {
        private static readonly Lazy<T> lazy = new Lazy<T>(() => new T());

        public static T Instance { get { return lazy.Value; } }
        public static bool IsCreated => lazy.IsValueCreated;

        private Singleton()
        {
        }
    }
}
