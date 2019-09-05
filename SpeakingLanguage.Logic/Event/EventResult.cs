using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public struct EventResult
    {
        public bool isSuccess;
        public string message;

        public EventResult(bool success, string msg = "")
        {
            isSuccess = success;
            message = msg;
        }
    }
}
