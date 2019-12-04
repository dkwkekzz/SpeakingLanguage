using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server.Networks
{
    internal sealed class Locator : Library.SingletonLazy<Locator>
    {
        public NetDataWriter NetBaseWriter { get; } = new NetDataWriter();
        public PostResponsor Responsor { get; } = new PostResponsor();
        public Authenticator Authenticator { get; } = new Authenticator();
        public IDatabase Database { get; } = new FileDatabase();
    }
}
