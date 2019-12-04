using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SpeakingLanguage.Server.Networks
{
    internal sealed class ServerListener : INetEventListener
    {
        private readonly PacketReceiver _receiver = new PacketReceiver();

        public void OnPeerConnected(NetPeer peer)
        {
            Library.Tracer.Write("[ServerListener] Peer connected: " + peer.EndPoint);
            _receiver.OnEnter(peer);
            //var peers = Server.GetPeers(ConnectionState.Connected);
            //foreach (var netPeer in peers)
            //{
            //    Console.WriteLine("ConnectedPeersList: id={0}, ep={1}", netPeer.Id, netPeer.EndPoint);
            //}
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Library.Tracer.Write("[ServerListener] Peer disconnected: " + peer.EndPoint + ", reason: " + disconnectInfo.Reason);
            _receiver.OnLeave(peer);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
        {
            Library.Tracer.Error("[ServerListener] error: " + socketErrorCode);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            Library.Tracer.Write($"[ServerListener] Peer reveived: {peer.EndPoint}, count: {reader.AvailableBytes.ToString()}");
            _receiver.OnReceive(peer, reader);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Library.Tracer.Write($"[ServerListener] ReceiveUnconnected: {reader.GetString(100)}");
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            var acceptedPeer = request.AcceptIfKey(Protocol.Constants.GAME_KEY);
            Library.Tracer.Write($"[ServerListener] ConnectionRequest. Ep: {request.RemoteEndPoint}, Accepted: {acceptedPeer != null}");
        }
    }
}
