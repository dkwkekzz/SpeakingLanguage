using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SpeakingLanguage.ClientWpf
{
    internal class PacketProcessor
    {
        private ClientListener _clientListener;
        private CancellationTokenSource _cts;

        public void Run(int port)
        {
            Task.Factory.StartNew(() =>
            {
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
                client1.Connect("127.0.0.1", port, Protocol.Define.GAME_KEY);

                //client2
                NetManager client2 = new NetManager(_clientListener)
                {
                    //SimulateLatency = true,
                    SimulationMaxLatency = 1500
                };

                client2.Start();
                client2.Connect("::1", port, Protocol.Define.GAME_KEY);

                var token = _cts.Token;
                while (!token.IsCancellationRequested)
                {
                    client1.PollEvents();
                    client2.PollEvents();
                    Thread.Sleep(15);
                }

                client1.Stop();
                client2.Stop();
                MessageBox.Show(string.Format("Client1Stats:\n BytesReceived: {0}\n PacketsReceived: {1}\n BytesSent: {2}\n PacketsSent: {3}",
                    client1.Statistics.BytesReceived,
                    client1.Statistics.PacketsReceived,
                    client1.Statistics.BytesSent,
                    client1.Statistics.PacketsSent), "Report");
                MessageBox.Show(string.Format("Client2Stats:\n BytesReceived: {0}\n PacketsReceived: {1}\n BytesSent: {2}\n PacketsSent: {3}",
                    client2.Statistics.BytesReceived,
                    client2.Statistics.PacketsReceived,
                    client2.Statistics.BytesSent,
                    client2.Statistics.PacketsSent), "Report");
            });
        }

        private void TestInput(NetPeer peer)
        {
            Task.Factory.StartNew(() =>
            {
                var writer = new NetDataWriter();
                var token = _cts.Token;
                while (!token.IsCancellationRequested)
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
                        case ConsoleKey.A:
                        case ConsoleKey.D:
                        case ConsoleKey.W:
                        case ConsoleKey.S:
                            writer.Put((int)Protocol.Code.Packet.Keyboard);
                            writer.Put(new Protocol.Packet.KeyboardData { press = true, key = (int)key });
                            break;
                        case ConsoleKey.D1:
                            writer.Put((int)Protocol.Code.Packet.SelectSubject);
                            writer.Put(new Protocol.Packet.ObjectData { handleValue = (int)key });
                            break;
                        case ConsoleKey.D2:
                            writer.Put((int)Protocol.Code.Packet.SubscribeScene);
                            writer.Put(new Protocol.Packet.SubscribeData { worldIndex = (int)key, count = 1 });
                            writer.Put(new Protocol.Packet.SceneData { sceneX = 0, sceneY = 0, sceneZ = 0 });
                            break;
                        case ConsoleKey.D3:
                            writer.Put((int)Protocol.Code.Packet.UnsubscribeScene);
                            writer.Put(new Protocol.Packet.SubscribeData { worldIndex = (int)key, count = 1, });
                            writer.Put(new Protocol.Packet.SceneData { sceneX = 0, sceneY = 0, sceneZ = 0 });
                            break;
                        case ConsoleKey.Q:
                            writer.Put((int)Protocol.Code.Packet.Interaction);
                            writer.Put(new Protocol.Packet.InteractionData
                            {
                                lhsValue = 0,
                                rhsValue = 1
                            });
                            break;
                        case ConsoleKey.Z:
                            writer.Put((int)Protocol.Code.Packet.Terminate);
                            //running = false;
                            break;
                    }
                    
                    peer.Send(writer, DeliveryMethod.ReliableOrdered);
                }
            });   
        }
    }
}
