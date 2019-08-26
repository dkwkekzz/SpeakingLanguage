using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SpeakingLanguage.Server
{
    internal class PacketProcessor
    {
        private ServerListener _serverListener;

        public void Run(ref Logic.StartInfo info)
        {
            Console.WriteLine("=== SpeakingLanguage Server ===");
            //Server
            _serverListener = new ServerListener();

            var server = new NetManager(_serverListener);
            if (!server.Start(info.port))
            {
                Console.WriteLine("Server start failed");
                Console.ReadKey();
                return;
            }
            _serverListener.Server = server;

            var receiver = new PacketReceiver(ref info);
            _serverListener.Receiver = receiver;

            while (!Console.KeyAvailable)
            {
                server.PollEvents();
                Thread.Sleep(15);
            }

            server.Stop();
            Console.ReadKey();
            Console.WriteLine("ServStats:\n BytesReceived: {0}\n PacketsReceived: {1}\n BytesSent: {2}\n PacketsSent: {3}",
                server.Statistics.BytesReceived,
                server.Statistics.PacketsReceived,
                server.Statistics.BytesSent,
                server.Statistics.PacketsSent);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
