using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace SpeakingLanguage.Server.Networks
{
    internal sealed class PacketProcessor : IDisposable
    {
        private ServerListener _serverListener;

        public PacketProcessor()
        {
        }

        public void Dispose()
        {
            if (_serverListener == null)
                return;

            var server = _serverListener.Server;
            server.Stop();

            Console.WriteLine("ServStats:\n BytesReceived: {0}\n PacketsReceived: {1}\n BytesSent: {2}\n PacketsSent: {3}",
                server.Statistics.BytesReceived,
                server.Statistics.PacketsReceived,
                server.Statistics.BytesSent,
                server.Statistics.PacketsSent);
        }

        public void Run(int port)
        {
            Console.WriteLine("=== SpeakingLanguage Server ===");

            //var timer = new Stopwatch();
            //timer.Start();
            _serverListener = new ServerListener();

            var server = new NetManager(_serverListener);
            if (!server.Start(port))
            {
                Console.WriteLine("Server start failed");
                Console.ReadKey();
                return;
            }
            _serverListener.Server = server;

            var receiver = new PacketReceiver();
            _serverListener.Receiver = receiver;

            var worldManager = WorldManager.Instance;
            var eventManager = Logic.EventManager.Instance;
            while (!Console.KeyAvailable)
            {
                eventManager.FrameEnter();

                server.PollEvents();
                worldManager.FlushUser();
                
                var ret = eventManager.ExecuteFrame();
                ret.Display();
                if (ret.Leg >= 0)
                    continue;
                
                Thread.Sleep(-ret.Leg);
            }
        }
    }
}
