using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SpeakingLanguage.Server.Network
{
    internal class PacketProcessor : IDisposable
    {
        private ServerListener _serverListener;

        public void Dispose()
        {
            var server = _serverListener.Server;
            server.Stop();

            Console.WriteLine("ServStats:\n BytesReceived: {0}\n PacketsReceived: {1}\n BytesSent: {2}\n PacketsSent: {3}",
                server.Statistics.BytesReceived,
                server.Statistics.PacketsReceived,
                server.Statistics.BytesSent,
                server.Statistics.PacketsSent);
        }

        public PacketProcessor(ref Logic.StartInfo info)
        {
            _serverListener = new ServerListener();

            var server = new NetManager(_serverListener);
            if (!server.Start(info.port))
            {
                Console.WriteLine("Server start failed");
                Console.ReadKey();
                return;
            }
            _serverListener.Server = server;

            var receiver = new PacketReceiver();
            _serverListener.Receiver = receiver;
        }

        public void Update()
        {
            _serverListener.Server.PollEvents();
        }
    }
}
