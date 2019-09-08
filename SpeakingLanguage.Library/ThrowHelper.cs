using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Library
{
    public static class ThrowHelper
    {
        public sealed class StateOverflowException : Exception
        {
            private string _msg;
            public override string Message => _msg;

            public StateOverflowException(string msg) { _msg = msg; }
        }

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

        public sealed class FailToConvertException : Exception
        {
            private string _msg;
            public override string Message => _msg;

            public FailToConvertException(string msg) { _msg = msg; }
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

        public static void ThrowStateOverflow(string msg = "")
        {
            throw new StateOverflowException(msg);
        }

        public static void ThrowWrongArgument(string msg = "")
        {
            throw new ArgumentException(msg);
        }

        public static void ThrowFailToConvert(string msg = "")
        {
            throw new ArgumentException(msg);
        }
    }
}
