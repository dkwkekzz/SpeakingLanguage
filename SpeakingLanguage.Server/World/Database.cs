using LiteNetLib;
using System;
using System.Collections.Generic;
using System.IO;

namespace SpeakingLanguage.Server
{
    internal class Database
    {
        private byte[] _buffer = new byte[1024];

        public async void ConstructUser(User agent, string id, string pswd)
        {
            // temp fileSystem
            if (File.Exists($"user:{id}"))
            {

            }
            else
            {
            }
        }

        public bool ConstructObject(int handleValue, out Library.Reader reader)
        {
            var writer = new Library.Writer(_buffer, 0);
            writer.WriteInt(0);
            writer.WriteInt(handleValue);

            reader = new Library.Reader(_buffer);
            return true;
        }
    }
}
