using LiteNetLib;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal struct Agent
    {
        public enum ESort
        {
            NPC = 0,
            PC,
        }

        public ESort Sort => Peer != null ? ESort.PC : ESort.NPC;
        public NetPeer Peer { get; private set; }
        public int Id { get; }
        public Logic.slObjectHandle SubjectHandle { get; private set; }

        public Agent(NetPeer peer)
        {
            Peer = peer;
            Id = Peer.Id;
            SubjectHandle = 0;
        }
    }
}
