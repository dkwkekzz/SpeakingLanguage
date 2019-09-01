using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public class Singleton<T>
        where T : class, new()
    {
        private static T _value = new T();
        public static T Instance => _value;

        protected Singleton()
        {
        }
    }
}
