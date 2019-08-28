using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SpeakingLanguage.Server.Network
{
    internal sealed class ServerListener : INetEventListener
    {
        public NetManager Server;
        public PacketReceiver Receiver;

        public void OnPeerConnected(NetPeer peer)
        {
            Console.WriteLine("[Server] Peer connected: " + peer.EndPoint);

            Receiver.OnEnter(peer);
            //var peers = Server.GetPeers(ConnectionState.Connected);
            //foreach (var netPeer in peers)
            //{
            //    Console.WriteLine("ConnectedPeersList: id={0}, ep={1}", netPeer.Id, netPeer.EndPoint);
            //}
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine("[Server] Peer disconnected: " + peer.EndPoint + ", reason: " + disconnectInfo.Reason);

            Receiver.OnLeave(peer);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
        {
            Console.WriteLine("[Server] error: " + socketErrorCode);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            Console.WriteLine($"[Server] Peer reveived: {peer.EndPoint}, count: {reader.AvailableBytes.ToString()}");

            Receiver.OnReceive(peer, reader);

            //fragment log
            if (reader.AvailableBytes == 13218)
            {
                Console.WriteLine("[Server] TestFrag: {0}, {1}",
                    reader.RawData[reader.UserDataOffset],
                    reader.RawData[reader.UserDataOffset + 13217]);
            }
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Console.WriteLine("[Server] ReceiveUnconnected: {0}", reader.GetString(100));
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {

        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            var acceptedPeer = request.AcceptIfKey("gamekey");
            Console.WriteLine("[Server] ConnectionRequest. Ep: {0}, Accepted: {1}",
                request.RemoteEndPoint,
                acceptedPeer != null);
        }
    }

}
