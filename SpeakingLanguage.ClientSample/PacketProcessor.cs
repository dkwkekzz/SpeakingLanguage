using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpeakingLanguage.ClientSample
{
    internal class PacketProcessor
    {
        private ClientListener _clientListener;
        private bool running = false;

        public void Run(int port)
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

            running = true;
            TestInput(new NetPeer[]{ client1.FirstPeer, client2.FirstPeer });

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

        private static void WriteGuide()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== SpeakingLanguage ClientSample ===");
            sb.AppendLine("=====================");
            sb.AppendLine("Format:{clientIdx} {반복횟수} {패킷번호} {data총개수} {data번호} {각데이터포맷별 값} {data2번호} ... ");
            sb.AppendLine("=====================");
            sb.AppendLine("패킷 번호");
            for (Protocol.Code.Packet code = 0; code != Protocol.Code.Packet.__MAX__; code++)
            {
                sb.AppendLine($"{((int)code).ToString()}. {code.ToString()}");
            }
            sb.AppendLine("=====================");
            sb.AppendLine("데이터 포맷");
            sb.AppendLine("1. KeyboardData -> format: {press:bool} {consoleKey:int}");
            sb.AppendLine("2. ObjectData -> format: {handleValue:int}");
            sb.AppendLine("3. SubscribeData -> format: {worldIndex:int} {count:int}");
            sb.AppendLine("4. SceneData -> format: {x:int} {y:int} {z:int}");
            sb.AppendLine("5. InteractionData -> format: {x:int} {y:int}");
            sb.AppendLine("6. AuthenticationData -> format: {id:string} {pswd:string}");
            sb.AppendLine("=====================");
            sb.AppendLine("기타: z: ");
            sb.AppendLine("=====================");
            Console.WriteLine(sb.ToString());
        }

        private void TestInput(NetPeer[] peers)
        {
            Task.Factory.StartNew(() =>
            {
                WriteGuide();

                var writer = new NetDataWriter();
                while (running)
                {
                    try
                    {
                        var command = Console.ReadLine();
                        var words = command.Split(' ');
                        var clientIdx = int.Parse(words[0]);
                        var peer = peers[clientIdx];

                        var repeat = int.Parse(words[1]);
                        for (int i = 0; i != repeat; i++)
                        {
                            writer.Reset();

                            var code = int.Parse(words[2]);
                            writer.Put(code);

                            var dataCnt = int.Parse(words[3]);
                            for (int j = 0; j != dataCnt; j++)
                            {
                                var dataNum = int.Parse(words[4]);
                                var startIndex = 5;
                                switch (dataNum)
                                {
                                    case 1:
                                        writer.Put(new Protocol.Packet.KeyboardData
                                        {
                                            press = int.Parse(words[startIndex + j * 2]) == 0 ? false : true,
                                            key = int.Parse(words[startIndex + j * 2 + 1])
                                        });
                                        break;
                                    case 2:
                                        if (words.Length > startIndex)
                                        {
                                            writer.Put(new Protocol.Packet.ObjectData
                                            { handleValue = int.Parse(words[startIndex + j]) });
                                        }
                                        else
                                        {
                                            writer.Put(new Protocol.Packet.ObjectData
                                            { handleValue = (i + 1) });
                                        }
                                        break;
                                    case 3:
                                        writer.Put(new Protocol.Packet.SubscribeData
                                        {
                                            worldIndex = int.Parse(words[startIndex + j * 2]),
                                            count = int.Parse(words[startIndex + j * 2 + 1])
                                        });
                                        break;
                                    case 4:
                                        writer.Put(new Protocol.Packet.SceneData
                                        {
                                            sceneX = int.Parse(words[startIndex + j * 3]),
                                            sceneY = int.Parse(words[startIndex + j * 3 + 1]),
                                            sceneZ = int.Parse(words[startIndex + j * 3 + 2])
                                        });
                                        break;
                                    case 5:
                                        writer.Put(new Protocol.Packet.InteractionData
                                        {
                                            lhsValue = int.Parse(words[startIndex + j * 2]),
                                            rhsValue = int.Parse(words[startIndex + j * 2 + 1])
                                        });
                                        break;
                                    case 6:
                                        writer.Put(new Protocol.Packet.AuthenticationData
                                        {
                                            id = words[startIndex + j * 2],
                                            pswd = words[startIndex + j * 2 + 1]
                                        });
                                        break;
                                }
                            }
                            
                            peer.Send(writer, DeliveryMethod.ReliableOrdered);
                        }
                    }
                    catch (IndexOutOfRangeException iex) { Console.WriteLine(iex.Message); }
                    catch (ArgumentOutOfRangeException aex) { Console.WriteLine(aex.Message); }
                    finally
                    {
                    }
                }
            });   
        }
    }
}
