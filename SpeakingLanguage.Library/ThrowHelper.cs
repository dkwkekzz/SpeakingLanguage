using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public static class ThrowHelper
    {
        public sealed class CapacityOverflowException : Exception
        {
            private string _msg;
            public override string Message => _msg;

            public CapacityOverflowException(string msg) { _msg = msg; }
        }

        public sealed class WrongStateException : Exception
        {
            private string _msg;
            public override string Message => _msg;

            public WrongStateException(string msg) { _msg = msg; }
        }
        
        public static void ThrowCapacityOverflow(string msg = "")
        {
            throw new CapacityOverflowException(msg);
        }

        public static void ThrowWrongState(string msg = "")
        {
            throw new WrongStateException(msg);
        }

        public static void ThrowKeyNotFound(string msg = "")
        {
            throw new KeyNotFoundException(msg);
        }

        public static void ThrowOutOfMemory(string msg = "")
        {
            throw new OutOfMemoryException(msg);
        }
    }
}
