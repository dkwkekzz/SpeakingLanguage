using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal class Scene
    {
        public enum ESide
        {
            Left,
            LeftTop,
            Top,
            TopRight,
            Right,
            RightBottom,
            Bottom,
            BottomLeft,
            Center,
        }

        public struct Visitor
        {
            public int handle;
        }

        public const int HORIZONTAL_CAPACITY = 1 << 8;
        public const int VERTICAL_CAPACITY = 1 << 8;
        public const int WIDTH = 1 << 10;
        public const int HEIGHT = 1 << 9;

        private Scene[] _sideRefs = new Scene[9];
        private Dictionary<int, WeakReference<Agent>> _subscriberDic = new Dictionary<int, WeakReference<Agent>>(16);
        private List<WeakReference<Agent>> _reservers = new List<WeakReference<Agent>>(8);
        private List<Visitor> _visitors = new List<Visitor>(16);
        private Library.Writer _stateWriter = new Library.Writer(1 << 10);

        public int Index { get; }
        public NetDataWriter DataWriter { get; } = new NetDataWriter();

        public Scene(int x_ofs, int y_ofs, int z_ofs, int w_ofs)
        {
            // 0000 0000 0000 0000 0000 0000 0000 0000 
            // x         y         z    w
            this.Index = x_ofs | (y_ofs << 8) | (z_ofs << 16) | (w_ofs << 20);
        }

        public void Link()
        {
            var sceneCol = World.Instance.SceneCollection;
            if (sceneCol.TryGetScene(this.Index - HORIZONTAL_CAPACITY - 1, out Scene ltscene))
                this._sideRefs[(int)ESide.LeftTop] = ltscene;
            if (sceneCol.TryGetScene(this.Index - HORIZONTAL_CAPACITY, out Scene tscene))
                this._sideRefs[(int)ESide.Top] = tscene;
            if (sceneCol.TryGetScene(this.Index - HORIZONTAL_CAPACITY + 1, out Scene rtscene))
                this._sideRefs[(int)ESide.TopRight] = rtscene;
            if (sceneCol.TryGetScene(this.Index - 1, out Scene lscene))
                this._sideRefs[(int)ESide.Left] = lscene;
            if (sceneCol.TryGetScene(this.Index + 1, out Scene rscene))
                this._sideRefs[(int)ESide.Right] = rscene;
            if (sceneCol.TryGetScene(this.Index + HORIZONTAL_CAPACITY - 1, out Scene lbscene))
                this._sideRefs[(int)ESide.BottomLeft] = lbscene;
            if (sceneCol.TryGetScene(this.Index + HORIZONTAL_CAPACITY, out Scene bscene))
                this._sideRefs[(int)ESide.Bottom] = bscene;
            if (sceneCol.TryGetScene(this.Index + HORIZONTAL_CAPACITY + 1, out Scene rbscene))
                this._sideRefs[(int)ESide.RightBottom] = rbscene;
            this._sideRefs[(int)ESide.Center] = this;
        }

        public void AddSubscriber(Agent agent)
        {
            for (int i = 0; i != 9; i++)
                this._sideRefs[i]._reservers.Add(new WeakReference<Agent>(agent));
        }

        public void RemoveSubscriber(Agent agent)
        {
            for (int i = 0; i != 9; i++)
                this._sideRefs[i]._subscriberDic.Remove(agent.Id);
        }

        public void AddVisitor(int handle)
        {
            _visitors.Add(new Visitor { handle = handle });
        }

        public void Notify()
        {
            var cntReserver = _reservers.Count;
            if (cntReserver != 0)
            {
                var cntVisitor = _visitors.Count;
                if (cntVisitor != 0)
                {
                    _stateWriter.WriteInt((int)Protocol.Code.Packet.Synchronization);
                    _stateWriter.WriteInt(cntVisitor);
                    for (int i = 0; i != cntVisitor; i++)
                    {
                        Logic.Synchronization.Serialize(_visitors[i].handle, ref _stateWriter);
                    }

                    for (int i = 0; i != cntReserver; i++)
                    {
                        if (!_reservers[i].TryGetTarget(out Agent agent))
                            continue;

                        agent.Send(_stateWriter.Buffer, 0, _stateWriter.Offset);
                        _subscriberDic[agent.Id] = _reservers[i];
                    }
                }
            }

            var lenData = this.DataWriter.Length;
            if (lenData > 4)
            {
                unsafe
                {
                    var checkIds = stackalloc int[_subscriberDic.Count];
                    var checkCnt = 0;

                    var subIter = _subscriberDic.Values.GetEnumerator();
                    while (subIter.MoveNext())
                    {
                        if (!subIter.Current.TryGetTarget(out Agent agent))
                        {
                            checkIds[checkCnt++] = agent.Id;
                            continue;
                        }

                        agent.Send(this.DataWriter.Data, 0, lenData);
                    }

                    for (int i = 0; i != checkCnt; i++)
                    {
                        _subscriberDic.Remove(checkIds[i]);
                    }
                }
            }

            this.DataWriter.Reset();
            this.DataWriter.Put((int)Protocol.Code.Packet.Subscribe);

            _reservers.Clear();
            _visitors.Clear();
            _stateWriter.Reset();
        }
    }

    internal class AuthData
    {
        public string id;
        public string pswd;
        public int handle;
    }

    internal class Agent
    {
        private NetPeer _peer;

        public AuthData Auth { get; set; }
        public Scene CurrentScene { get; set; }
        public Logic.PropertyTable.Row LogicObject { get; set; }

        public int Id => _peer.Id;
        public bool Authenticated => this.Auth != null;

        public Agent(NetPeer peer)
        {
            _peer = peer;
        }

        public void Send(byte[] data, int ofs, int cnt, byte channel = 0)
        {
            _peer.Send(data, ofs, cnt, channel, DeliveryMethod.ReliableOrdered);
        }
    }
}
