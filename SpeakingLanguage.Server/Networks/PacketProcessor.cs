using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace SpeakingLanguage.Server.Networks
{
    internal sealed class PacketProcessor : IDisposable
    {
        public PacketProcessor()
        {
        }

        public void Dispose()
        {
        }

        public void Run(int port)
        {
            var server = new NetManager(new ServerListener());
            if (!server.Start(port))
            {
                Console.WriteLine("Server start failed");
                Console.ReadKey();
                return;
            }

            while (!Console.KeyAvailable)
            {
                server.PollEvents();

                var ret = eventManager.ExecuteFrame();
                if (ret.Leg >= 0)
                    continue;

                Thread.Sleep(-ret.Leg);
            }

            server.Stop();

            Console.WriteLine("ServStats:\n BytesReceived: {0}\n PacketsReceived: {1}\n BytesSent: {2}\n PacketsSent: {3}",
                server.Statistics.BytesReceived,
                server.Statistics.PacketsReceived,
                server.Statistics.BytesSent,
                server.Statistics.PacketsSent);
        }
    }
}
