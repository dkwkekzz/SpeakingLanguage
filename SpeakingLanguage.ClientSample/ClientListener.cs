using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SpeakingLanguage.ClientSample
{
    internal class ClientListener : INetEventListener
    {
        private static int _messagesReceivedCount;

        public void OnPeerConnected(NetPeer peer)
        {
            Console.WriteLine("[Client] connected to: {0}:{1}", peer.EndPoint.Address, peer.EndPoint.Port);

            //NetDataWriter dataWriter = new NetDataWriter();
            //for (int i = 0; i < 5; i++)
            //{
            //    dataWriter.Reset();
            //    dataWriter.Put(0);
            //    dataWriter.Put(i);
            //    peer.Send(dataWriter, DeliveryMethod.ReliableUnordered);
            //
            //    dataWriter.Reset();
            //    dataWriter.Put(1);
            //    dataWriter.Put(i);
            //    peer.Send(dataWriter, DeliveryMethod.ReliableOrdered);
            //
            //    dataWriter.Reset();
            //    dataWriter.Put(2);
            //    dataWriter.Put(i);
            //    peer.Send(dataWriter, DeliveryMethod.Sequenced);
            //
            //    dataWriter.Reset();
            //    dataWriter.Put(3);
            //    dataWriter.Put(i);
            //    peer.Send(dataWriter, DeliveryMethod.Unreliable);
            //
            //    dataWriter.Reset();
            //    dataWriter.Put(4);
            //    dataWriter.Put(i);
            //    peer.Send(dataWriter, DeliveryMethod.ReliableSequenced);
            //}
            //
            ////And test fragment
            //byte[] testData = new byte[13218];
            //testData[0] = 192;
            //testData[13217] = 31;
            //peer.Send(testData, DeliveryMethod.ReliableOrdered);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            var code = Protocol.Code.Packet.None;
            var srcCode = Protocol.Code.Packet.None;
            var error = Protocol.Code.Error.None;
            var logicError = Logic.EventError.None;
            if (disconnectInfo.AdditionalData.AvailableBytes > 0)
            {
                code = (Protocol.Code.Packet)disconnectInfo.AdditionalData.GetInt();
                srcCode = (Protocol.Code.Packet)disconnectInfo.AdditionalData.GetInt();
                error = (Protocol.Code.Error)disconnectInfo.AdditionalData.GetInt();
                logicError = (Logic.EventError)disconnectInfo.AdditionalData.GetInt();
            }
            
            Console.WriteLine($"[Client] disconnected: {peer.EndPoint}, code: {code}, error: {error}, logicError: {logicError}, reason: {disconnectInfo.Reason}");
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
        {
            Console.WriteLine("[Client] error! " + socketErrorCode);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            if (reader.AvailableBytes == 13218)
            {
                Console.WriteLine("[{0}] TestFrag: {1}, {2}",
                    peer.NetManager.LocalPort,
                    reader.RawData[reader.UserDataOffset],
                    reader.RawData[reader.UserDataOffset + 13217]);
            }
            else
            {
                if (reader.AvailableBytes <= 0)
                    return;

                var code = (Protocol.Code.Packet)reader.GetInt();
                _messagesReceivedCount++;
                Console.WriteLine("[{0}] CNT: {1}, CODE: {2}, MTD: {3}", peer.NetManager.LocalPort, _messagesReceivedCount, code.ToString(), deliveryMethod);

                switch (code)
                {
                    case Protocol.Code.Packet.CreateSubject:
                        var data = reader.Get<Protocol.Packet.ObjectData>();
                        Console.WriteLine($"[{code}] handle: {data.handleValue.ToString()}");
                        break;
                }
            }
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {

        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {

        }

        public void OnConnectionRequest(ConnectionRequest request)
        {

        }
    }
}
