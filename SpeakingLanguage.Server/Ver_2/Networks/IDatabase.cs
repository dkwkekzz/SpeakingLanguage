using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server.Networks
{
    internal interface IDatabase : IDisposable
    {
        void RequestReadUser(User agent, string id, string pswd);
        void RequestWriteUser(User agent, string fileKey);
        //void RequestReadObject(User agent, long handleValue);
        //void RequestWriteObject(User agent, long objUid);
        void FlushResponse();
    }
}
