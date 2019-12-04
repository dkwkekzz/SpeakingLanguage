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

        public PostResponsor()
        {
            _responses = new ConcurrentQueue<Response>();
            _responseActions = new PostAction[(int)Protocol.Code.Packet.__MAX__];
            _responseActions[(int)Protocol.Code.Packet.Authentication] = _onAuthentication;
        }

        public void Enqueue(Response response)
        {
            _responses.Enqueue(response);
        }

        public void Pull()
        {
            while (_responses.TryDequeue(out Response res))
            {
                if (res.err != Protocol.Code.Error.None)
                {
                    PacketHelper.CastFailure(res.agent, res.from, res.err);
                    continue;
                }

                var ret = _responseActions[(int)res.from](ref res);
                if (!ret.IsSuccess)
                {
                    PacketHelper.CastFailure(res.agent, res.from, ret.Error);
                    continue;
                }
            }
        }

        private Result _onAuthentication(ref Response response)
        {
            var agent = response.agent;
            agent.Auth = response.res as AuthData;
            agent.LogicObject = Logic.Factory.CreateNew(agent.Auth.handle, Logic.AccountArchetype.Value);

            var writer = Locator.Instance.NetBaseWriter;
            writer.Reset();
            writer.Put((int)Protocol.Code.Packet.Authentication);
            writer.Put(agent.Auth.id);
            writer.Put(agent.Auth.pswd);
            writer.Put(agent.Auth.handle);
            agent.Send(writer.Data, 0, writer.Length);

            return new Result();
        }
    }
}
