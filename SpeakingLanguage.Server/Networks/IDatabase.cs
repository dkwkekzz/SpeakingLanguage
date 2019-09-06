using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server.Networks
{
    internal interface IDatabase
    {
        void ConstructUser(User agent, string id, string pswd);
        bool ConstructObject(int handleValue, out Library.Reader reader);
    }
}
