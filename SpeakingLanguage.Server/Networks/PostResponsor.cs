using LiteNetLib.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SpeakingLanguage.Server.Networks
{
    internal enum Caller
    {
        Authenticator,
        FileDatabase,
    }

    internal struct Response
    {
        public Protocol.Code.Packet from;
        public Protocol.Code.Error err;
        public Caller caller;
        public Agent agent;
        public object res;
    }

    internal class PostResponsor
    {
        private readonly ConcurrentQueue<Response> _responses;
        private delegate Result PostAction(ref Response response);
        private readonly PostAction[] _responseActions;
        private readonly NetDataWriter _dataWriter;

        public PostResponsor()
        {
            _responses = new ConcurrentQueue<Response>();
            _dataWriter = new NetDataWriter();
            _responseActions = new PostAction[(int)Protocol.Code.Packet.__MAX__];
            _responseActions[(int)Protocol.Code.Packet.Authentication] = _onAuthentication;
            _responseActions[(int)Protocol.Code.Packet.Select] = _onSelect;
            _responseActions[(int)Protocol.Code.Packet.Disconnection] = _onDisconnection;
            _responseActions[(int)Protocol.Code.Packet.Subscribe] = _onSubscribe;
            _responseActions[(int)Protocol.Code.Packet.Control] = _onControl;
            _responseActions[(int)Protocol.Code.Packet.Interaction] = _onInteraction;
        }

        public void Enqueue(Response response)
        {
            _responses.Enqueue(response);
        }

        public void Flush()
        {
            while (_responses.TryDequeue(out Response res))
            {
                if (res.err != Protocol.Code.Error.None)
                {
                    PacketHelper.CastFailure(res.agent, _dataWriter, res.from, res.err);
                    continue;
                }

                var ret = _responseActions[(int)res.from](ref res);
                if (!ret.IsSuccess)
                {
                    PacketHelper.CastFailure(res.agent, _dataWriter, res.from, ret.Error);
                    continue;
                }
            }
        }

        private Result _onAuthentication(ref Response response)
        {
            var agent = response.agent;
            agent.Auth = response.res as AuthData;
            agent.LogicObject = Logic.Factory.CreateNew(agent.Auth.handle, Logic.AccountArchetype.Value);

            _dataWriter.Reset();
            _dataWriter.Put((int)Protocol.Code.Packet.Authentication);
            _dataWriter.Put(new Protocol.Packet.S2C.Authentication
            {
                id = agent.Auth.id,
                pswd = agent.Auth.pswd,
                handle = agent.Auth.handle,
            });
            agent.Send(_dataWriter.Data, 0, _dataWriter.Length);
            return new Result();
        }

        private Result _onSelect(ref Response response)
        {
            //var agent = response.agent;
            //var rawData = response.res as byte[];
            //var reader = new Library.Reader(rawData);
            //var handle = Logic.Synchronization.Deserialize(ref reader);
            //if (handle < 0)
            //    return new Result(Protocol.Code.Error.FailToDeserialize);
            //
            //agent.SelectSubject(handle);
            //
            //agent.DataWriter.Put((int)Protocol.Code.Packet.Synchronization);
            //agent.DataWriter.Put(new Protocol.Packet.S2C.Synchronization { length = rawData.Length, rawData = rawData });
            return new Result();
        }

        private Result _onDisconnection(ref Response response)
        {
            //var agent = response.agent;
            //int handle;
            //unsafe
            //{
            //    var subject = Logic.PropertyHelper.Get<Logic.Subject>(agent.Properties);
            //    if (subject == null)
            //        return new Result(Protocol.Code.Error.NullReferenceObject);
            //
            //    handle = subject->handle;
            //}
            //
            //var pt = Logic.Locator.Instance.PropertyTable;
            //pt.Remove(handle);
            //
            //var agentCol = World.Instance.AgentCollection;
            //agentCol.Remove(response.agent.Id);
            return new Result();
        }

        private Result _onInteraction(ref Response response)
        {
            return new Result();
        }

        private Result _onSubscribe(ref Response response)
        {
            return new Result();
        }

        private Result _onControl(ref Response response)
        {
            return new Result();
        }
    }
}
