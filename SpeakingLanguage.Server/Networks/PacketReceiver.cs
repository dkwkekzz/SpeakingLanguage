﻿using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server.Networks
{
    internal class PacketReceiver
    {
        private readonly Authenticator _authenticator;
        private readonly IDatabase _database;
        private readonly Func<Agent, NetPacketReader, Result>[] _packetActions;
        private readonly NetDataWriter _dataWriter;

        public PacketReceiver(Authenticator auth, IDatabase database)
        {
            _authenticator = auth;
            _database = database;
            _dataWriter = new NetDataWriter();
            _packetActions = new Func<Agent, NetPacketReader, Result>[(int)Protocol.Code.Packet.__MAX__];
            _packetActions[(int)Protocol.Code.Packet.Authentication] = _onAuthentication;
            _packetActions[(int)Protocol.Code.Packet.Synchronization] = _onSynchronization;
            _packetActions[(int)Protocol.Code.Packet.Select] = _onSelect;
            _packetActions[(int)Protocol.Code.Packet.Subscribe] = _onSubscribe;
            _packetActions[(int)Protocol.Code.Packet.Control] = _onControl;
            _packetActions[(int)Protocol.Code.Packet.Interaction] = _onInteraction;
        }

        public void OnEnter(NetPeer peer)
        {
            var agentCol = World.Instance.AgentCollection;

            var id = peer.Id;
            if (agentCol.Contains(id))
                Library.ThrowHelper.ThrowWrongArgument("Duplicate agent id on enter.");

            agentCol.Insert(peer);
        }

        public void OnLeave(NetPeer peer)
        {
            var agentCol = World.Instance.AgentCollection;

            var id = peer.Id;
            if (!agentCol.TryGetAgent(id, out Agent agent))
                Library.ThrowHelper.ThrowWrongArgument("Not found agent id on leave.");

            //_database.RequestSave(agent, Protocol.Code.Packet.Disconnection, AgentHelper.GenerateDataKey(agent.Auth.id, agent.Auth.pswd));
        }

        public void OnReceive(NetPeer peer, NetPacketReader reader)
        {
            var agentCol = World.Instance.AgentCollection;

            var id = peer.Id;
            if (!agentCol.TryGetAgent(id, out Agent agent))
                Library.ThrowHelper.ThrowWrongArgument("Not found agent id on receive.");

            var code = reader.GetInt();
            var res = _packetActions[code](agent, reader);
            if (!res.IsSuccess)
            {
                PacketHelper.CastFailure(agent, _dataWriter, (Protocol.Code.Packet)code, res.Error);
                return;
            }

            Library.Tracer.Write($"[CastSuccess] id: {peer.Id.ToString()}, code: {((Protocol.Code.Packet)code).ToString()}");
        }

        private Result _onAuthentication(Agent agent, NetPacketReader reader)
        {
            if (agent.Authenticated)
                return new Result(Protocol.Code.Error.DuplicateAuthentication);

            var data = reader.Get<Protocol.Packet.Authentication>();
            _authenticator.Check(agent, Protocol.Code.Packet.Authentication, data.id, data.pswd);
            return new Result();
        }

        private Result _onSynchronization(Agent agent, NetPacketReader reader)
        {
            //if (!agent.Authenticated)
            //    return new Result(Protocol.Code.Error.InvaildAuthentication);
            //
            //var data = reader.Get<Protocol.Packet.Synchronization>();
            //var rawData = Logic.Synchronization.Serialize(data.handle);
            //if (null == rawData)
            //    return new Result(Protocol.Code.Error.NullReferenceObject);
            //
            //agent.DataWriter.Put((int)Protocol.Code.Packet.Synchronization);
            //agent.DataWriter.Put(new Protocol.Packet.S2C.Synchronization { length = rawData.Length, rawData = rawData });
            return new Result();
        }

        private Result _onSelect(Agent agent, NetPacketReader reader)
        {
            //if (!agent.Authenticated)
            //    return new Result(Protocol.Code.Error.InvaildAuthentication);
            //
            //var data = reader.Get<Protocol.Packet.Select>();
            //
            //_database.RequestLoad(agent, Protocol.Code.Packet.Select, 
            //    $"{agent.Auth.id}_{agent.Auth.pswd}_{data.subjectKey.ToString()}.slo");
            return new Result();
        }

        private Result _onSubscribe(Agent agent, NetPacketReader reader)
        {
            //if (!agent.Authenticated)
            //    return new Result(Protocol.Code.Error.InvaildAuthentication);
            //
            //var data = reader.Get<Protocol.Packet.Subscribe>();
            //var sceneCol = World.Instance.SceneCollection;
            //if (!sceneCol.TryGetScene(data.sceneIndex, out Scene scene))
            //    return new Result(Protocol.Code.Error.NullReferenceScene);
            //
            //agent.CurrentScene.RemoveSubscriber(agent);
            //agent.CurrentScene = scene;
            //agent.CurrentScene.AddSubscriber(agent);

            return new Result();
        }

        private Result _onControl(Agent agent, NetPacketReader reader)
        {
            if (!agent.Authenticated)
                return new Result(Protocol.Code.Error.InvaildAuthentication);

            var data = reader.Get<Protocol.Packet.Control>();
            agent.CurrentScene.DataWriter.Put((int)Protocol.Code.Packet.Control);
            agent.CurrentScene.DataWriter.Put(data);

            // ...

            return new Result();
        }

        private Result _onInteraction(Agent agent, NetPacketReader reader)
        {
            if (!agent.Authenticated)
                return new Result(Protocol.Code.Error.InvaildAuthentication);

            var scene = ReceiverHelper.GetCurrentScene(_world, agent);
            if (null == scene)
                return new Result(Protocol.Code.Error.NullReferenceCollider);

            var data = reader.Get<Protocol.Packet.Interaction>();

            var eventManager = Logic.EventManager.Instance;
            var eRes = eventManager.InsertInteraction(data.lhsValue, data.rhsValue);
            if (!eRes.Success)
                return new Result(Protocol.Code.Error.FailToInteraction, eRes.error);

            var sceneIter = scene.GetEnumerator();
            while (sceneIter.MoveNext())
            {
                var subscriber = sceneIter.Current;
                subscriber.DataWriter.Put((int)Protocol.Code.Packet.Interaction);
                subscriber.DataWriter.Put(data);
            }

            return new Result();
        }

        private Result _onConstructIdentifier(Agent agent, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.GetUser(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);

            if (agent.Authentication)
                return new Result(Protocol.Code.Error.AlreadyConstructed);

            var data = reader.Get<Protocol.Packet.Authentication>();
            if (!ReceiverHelper.ValidateAuthenticate(ref data))
                return new Result(Protocol.Code.Error.FailToAuthentication);

            var writer = new Library.Writer(_serializeBuffer, 0);
            writer.WriteString(data.id);
            writer.WriteString(data.pswd);
            writer.WriteLong(Library.Ticker.GlobalTicks);
            writer.WriteInt(0);

            var reader2 = new Library.Reader(_serializeBuffer, writer.Offset);
            agent.OnDeserialize(ref reader2);

            return new Result();
        }

        private Result _onTerminate(NetPeer peer, NetPacketReader reader)
        {
            return new Result();
        }

        private Result _onKeyboard(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.GetUser(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);

            var scene = ReceiverHelper.GetCurrentScene(_world, agent);
            if (null == scene)
                return new Result(Protocol.Code.Error.NullReferenceCollider);

            var data = reader.Get<Protocol.Packet.Control>();

            var eventManager = Logic.EventManager.Instance;
            var eRes = eventManager.InsertKeyboard(agent.SubjectHandle.value, data.key, data.press ? 1 : 0);
            if (!eRes.Success)
                return new Result(Protocol.Code.Error.NullReferenceSubjectHandle, eRes.error);

            var sceneIter = scene.GetEnumerator();
            while (sceneIter.MoveNext())
            {
                var subscriber = sceneIter.Current;
                subscriber.DataWriter.Put((int)Protocol.Code.Packet.Keyboard);
                subscriber.DataWriter.Put(data);
            }
            
            return new Result();
        }

        private Result _onTouch(NetPeer peer, NetPacketReader reader)
        {
            return new Result();
        }

        private Result _onSelectSubject(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.GetUser(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);

            if (!agent.Authentication)
                return new Result(Protocol.Code.Error.InvalidAuthentication);

            var data = reader.Get<Protocol.Packet.Synchronization>();

            var selector = agent.SubjectSelector;
            if (!selector.TryCaptureSubject(data.handle, out long uid))
                return new Result(Protocol.Code.Error.NullReferenceSubjectHandle);

            agent.DataWriter.Put((int)Protocol.Code.Packet.SelectSubject);
            agent.DataWriter.Put(new Protocol.Packet.Synchronization { handle = data.handle });

            return new Result();
        }

        private Result _onCreateSubject(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.GetUser(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);

            if (!agent.Authentication)
                return new Result(Protocol.Code.Error.InvalidAuthentication);

            var objUid = ReceiverHelper.CreateObjectUid(id);
            var eRes = Logic.EventManager.Instance.CreateObject();
            if (!eRes.Success)
                return new Result(Protocol.Code.Error.NullReferenceSubjectHandle, eRes.error); 

            var objHandle = eRes.result;
            var selector = agent.SubjectSelector;
            selector.InsertSubject(objUid, objHandle);
            
            agent.DataWriter.Put((int)Protocol.Code.Packet.CreateSubject);
            agent.DataWriter.Put(new Protocol.Packet.Synchronization { handle = objHandle.value });

            return new Result();
        }

        private Result _onSubscribeScene(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.GetUser(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);

            var eventManager = Logic.EventManager.Instance;

            var writer = new Library.Writer(_serializeBuffer, 0);
            var subData = reader.Get<Protocol.Packet.Subscribe>();
            for (int i = 0; i != subData.count; i++)
            {
                var data = reader.Get<Protocol.Packet.SceneData>();
                var scene = _world.Scenes.Get(new SceneHandle(subData.sceneIndex, data));
                if (!scene.TryAddNotification(agent))
                {
                    scene = _world.Scenes.ExpandScene(scene);
                    scene.TryAddNotification(agent);
                }

                scene = agent.SubscribeScene(scene);
                if (null == scene)
                    return new Result(Protocol.Code.Error.OverflowSubscribe);

                var sceneIter = scene.GetEnumerator();
                while (sceneIter.MoveNext())
                {
                    var subjectHandle = sceneIter.Current.SubjectHandle;
                    var eRes = eventManager.SerializeObject(subjectHandle, ref writer);
                    if (!eRes.Success)
                        return new Result(Protocol.Code.Error.FailToSerialize, eRes.error);
                }
            }
            writer.WriteInt(0); // for null subject

            agent.DataWriter.Put((int)Protocol.Code.Packet.SubscribeScene);
            agent.DataWriter.Put(writer.Buffer, 0, writer.Offset);

            return new Result();
        }

        private Result _onUnsubscribeScene(NetPeer peer, NetPacketReader reader)
        {
            var id = peer.Id;
            var agent = _world.Agents.GetUser(id);
            if (agent == null)
                return new Result(Protocol.Code.Error.NullReferenceAgent);
            
            agent.DataWriter.Put((int)Protocol.Code.Packet.UnsubscribeScene);
            
            var subData = reader.Get<Protocol.Packet.Subscribe>();
            for (int i = 0; i != subData.count; i++)
            {
                var data = reader.Get<Protocol.Packet.SceneData>();
                var scene = _world.Scenes.Get(new SceneHandle(subData.sceneIndex, data));
                if (!agent.UnsubscribeScene(scene))
                    return new Result(Protocol.Code.Error.NullReferenceScene);

                if (!scene.CancelNotification(id))
                    return new Result(Protocol.Code.Error.NullReferenceSubsrciber);

                var subCount = scene.Count;
                if (subCount != 0)
                {
                    var sceneIter = scene.GetEnumerator();
                    while (sceneIter.MoveNext())
                    {
                        var subjectHandle = sceneIter.Current.SubjectHandle;
                        agent.DataWriter.Put(new Protocol.Packet.Synchronization { handle = subjectHandle.value });
                    }
                }
                else
                {
                    _world.Scenes.Remove(new SceneHandle(subData.sceneIndex, data));
                }
            }

            agent.DataWriter.Put(new Protocol.Packet.Synchronization { handle = 0 });   // for null subject
            
            return new Result();
        }
    }
}
