using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SpeakingLanguage.Server.Networks
{
    internal class Authenticator
    {
        private readonly PostResponsor _responses;

        public Authenticator(PostResponsor responsor)
        {
            _responses = responsor;
        }

        public void Check(Agent agent, Protocol.Code.Packet from, string id, string pswd)
        {
            var handle = World.Instance.KeyGenerator.NewHandle;
            _responses.Enqueue(new Response 
            { 
                agent = agent, 
                from = from, 
                caller = Caller.Authenticator,
                res = new AuthData { id = id, pswd = pswd, handle = handle } 
            });
        }
    }
}
