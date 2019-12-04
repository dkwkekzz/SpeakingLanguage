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

        private readonly Scene[] _sideRefs = new Scene[9];
        private readonly Dictionary<int, WeakReference<Agent>> _subscriberDic = new Dictionary<int, WeakReference<Agent>>(16);
        private readonly List<WeakReference<Agent>> _reservers = new List<WeakReference<Agent>>(8);
        private readonly List<Visitor> _visitors = new List<Visitor>(16);

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

        public int SubscriberCount => _subscriberDic.Count;
        public int VisitorCount => _visitors.Count;

        public Dictionary<int, WeakReference<Agent>>.ValueCollection.Enumerator Subscribers
            => _subscriberDic.Values.GetEnumerator();

        public List<WeakReference<Agent>>.Enumerator Reservers
            => _reservers.GetEnumerator();

        public List<Visitor>.Enumerator Visitors
            => _visitors.GetEnumerator();

        public void Reset()
        {
            _visitors.Clear();
            _reservers.Clear();
        }

        public void ReserveSubscriber(Agent agent)
        {
            for (int i = 0; i != 9; i++)
                this._sideRefs[i]._reservers.Add(new WeakReference<Agent>(agent));
        }

        public void CancelSubscriber(Agent agent)
        {
            for (int i = 0; i != 9; i++)
                this._sideRefs[i]._subscriberDic.Remove(agent.Id);
        }

        public void AddSubscriber(int id, WeakReference<Agent> wAgent)
        {
            _subscriberDic[id] = wAgent;
        }

        public void RemoveSubscriber(int id)
        {
            _subscriberDic.Remove(id);
        }

        public void AddVisitor(int handle)
        {
            _visitors.Add(new Visitor { handle = handle });
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
        private Logic.PropertyTable.Row _logicObject;

        public AuthData Auth { get; private set; }
        public Scene CurrentScene { get; set; }
        public Logic.Property.Controller LogicController { get; private set; }
        public Logic.Property.Position LogicPosition { get; private set; }

        public int Id => _peer.Id;
        public bool IsAlive => this.Auth != null;

        public Agent(NetPeer peer)
        {
            _peer = peer;
        }

        public void Construct(string id, string pswd, int newHandle)
        {
            this.Auth = new AuthData { id = id, pswd = pswd, handle = newHandle };
            this._logicObject = Logic.Factory.CreateNew(newHandle, Logic.ObserverArchetype.Value);
        }

        public void LogicUpdate()
        {
            this.LogicController = Logic.PropertyHelper.GetReadonly<Logic.Property.Controller>(this._logicObject);
            this.LogicPosition = Logic.PropertyHelper.GetReadonly<Logic.Property.Position>(this._logicObject);
        }

        public void Send(byte[] data, int ofs, int cnt, byte channel = 0)
        {
            _peer.Send(data, ofs, cnt, channel, DeliveryMethod.ReliableOrdered);
        }
    }
}
