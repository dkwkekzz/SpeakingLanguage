using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SpeakingLanguage.ClientSample
{
    internal class PacketProcessor
    {
        private ClientListener _clientListener;
        private bool running = false;

        public void Run(ref Logic.StartInfo info)
        {
            Console.WriteLine("=== SpeakingLanguage ClientSample ===");

            //Client
            _clientListener = new ClientListener();

            NetManager client1 = new NetManager(_clientListener)
            {
                SimulationMaxLatency = 1500,
                //SimulateLatency = true,
            };
            //client1
            if (!client1.Start())
            {
                Console.WriteLine("Client1 start failed");
                return;
            }
            client1.Connect("127.0.0.1", info.port, "gamekey");

            //client2
            NetManager client2 = new NetManager(_clientListener)
            {
                //SimulateLatency = true,
                SimulationMaxLatency = 1500
            };

            client2.Start();
            client2.Connect("::1", info.port, "gamekey");

            running = true;
            TestInput(client1.FirstPeer);

            while (running)
            {
                client1.PollEvents();
                client2.PollEvents();
                Thread.Sleep(15);
            }

            client1.Stop();
            client2.Stop();
            Console.ReadKey();
            Console.WriteLine("Client1Stats:\n BytesReceived: {0}\n PacketsReceived: {1}\n BytesSent: {2}\n PacketsSent: {3}",
                client1.Statistics.BytesReceived,
                client1.Statistics.PacketsReceived,
                client1.Statistics.BytesSent,
                client1.Statistics.PacketsSent);
            Console.WriteLine("Client2Stats:\n BytesReceived: {0}\n PacketsReceived: {1}\n BytesSent: {2}\n PacketsSent: {3}",
                client2.Statistics.BytesReceived,
                client2.Statistics.PacketsReceived,
                client2.Statistics.BytesSent,
                client2.Statistics.PacketsSent);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private void TestInput(NetPeer peer)
        {
            Task.Factory.StartNew(() =>
            {
                var writer = new NetDataWriter();
                while (running)
                {
                    writer.Reset();

                    var input = Console.ReadKey();
                    var key = input.Key;
                    switch (input.Key)
                    {
                        case ConsoleKey.LeftArrow:
                        case ConsoleKey.RightArrow:
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.DownArrow:
                            writer.Put((int)Protocol.Code.Packet.Keyboard);
                            writer.Put(new Protocol.Packet.KeyboardData { press = true, key = (int)key });
                            break;
                        case ConsoleKey.A:
                        case ConsoleKey.D:
                        case ConsoleKey.W:
                        case ConsoleKey.S:
                            writer.Put((int)Protocol.Code.Packet.SelectScene);
                            writer.Put(new Protocol.Packet.SceneData { worldIndex = (int)key });
                            break;
                        case ConsoleKey.D1:
                            writer.Put((int)Protocol.Code.Packet.SubscribeScene);
                            writer.Put(new Protocol.Packet.SceneData { worldIndex = (int)key });
                            break;
                        case ConsoleKey.D2:
                            writer.Put((int)Protocol.Code.Packet.UnsubscribeScene);
                            writer.Put(new Protocol.Packet.SceneData { worldIndex = (int)key });
                            break;
                        case ConsoleKey.Q:
                            writer.Put((int)Protocol.Code.Packet.Interaction);
                            writer.Put(new Protocol.Packet.InteractionData
                            {
                                lhsValue = new Protocol.Packet.InteractionData.ObjectHandle { value = 0 },
                                rhsValue = new Protocol.Packet.InteractionData.ObjectHandle { value = 1 }
                            });
                            break;
                        case ConsoleKey.Z:
                            writer.Put((int)Protocol.Code.Packet.Terminate);
                            running = false;
                            break;
                    }
                    
                    peer.Send(writer, DeliveryMethod.ReliableOrdered);
                }
            });   
        }
    }
}
