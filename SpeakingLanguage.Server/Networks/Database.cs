using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SpeakingLanguage.Server.Networks
{
    internal interface IDatabase
    {
        void RequestLoad(Agent agent, Protocol.Code.Packet from, string key);
        void RequestSave(Agent agent, Protocol.Code.Packet from, string key, byte[] savedData);
    }

    internal sealed class FileDatabase : IDatabase
    {
        private struct ReadRequest
        {
            public Agent agent;
            public Protocol.Code.Packet from;
            public string fileKey;
        }

        private struct WriteRequest
        {
            public Agent agent;
            public Protocol.Code.Packet from;
            public string fileKey;
            public byte[] rawData;
        }

        private readonly ConcurrentQueue<ReadRequest> _readRequests = new ConcurrentQueue<ReadRequest>();
        private readonly ConcurrentQueue<WriteRequest> _writeRequests = new ConcurrentQueue<WriteRequest>();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private Task _readWorker;
        private Task _writeWorker;

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        public void RequestLoad(Agent agent, Protocol.Code.Packet from, string key)
        {
            _readRequests.Enqueue(new ReadRequest { agent = agent, from = from, fileKey = key });
            _readAsync();
        }

        public void RequestSave(Agent agent, Protocol.Code.Packet from, string key, byte[] savedData)
        {
            _writeRequests.Enqueue(new WriteRequest { agent = agent, from = from, fileKey = key, rawData = savedData });
            _writeAsync();
        }

        private void _readAsync()
        {
            if (_readWorker != null) return;

            _readWorker = Task.Factory.StartNew((object obj) =>
            {
                CancellationToken token = (CancellationToken)obj;
                while (!token.IsCancellationRequested && _readRequests.Count > 0)
                {
                    if (!_readRequests.TryDequeue(out ReadRequest req)) continue;

                    string folderName = @"FileDatabase";
                    string pathString = Path.Combine(folderName, req.fileKey);

                    Directory.CreateDirectory(folderName);

                    try
                    {
                        byte[] rawData = null;
                        if (File.Exists(pathString))
                        {
                            rawData = File.ReadAllBytes(pathString);
                        }

                        _responses.Enqueue(new Response 
                        { 
                            from = req.from, 
                            err = Protocol.Code.Error.None,
                            caller = Caller.FileDatabase,
                            agent = req.agent,
                            res = rawData 
                        });
                    }
                    catch (IOException e)
                    {
                        Library.Tracer.Exception(this, e);
                    }
                }
                _readWorker = null;
            }, _cts.Token);
        }

        private void _writeAsync()
        {
            if (_writeWorker != null) return;

            _writeWorker = Task.Factory.StartNew((object obj) =>
            {
                CancellationToken token = (CancellationToken)obj;
                while (!token.IsCancellationRequested && _writeRequests.Count > 0)
                {
                    if (!_writeRequests.TryDequeue(out WriteRequest req)) continue;

                    string folderName = @"FileDatabase";
                    string pathString = Path.Combine(folderName, req.fileKey);

                    Directory.CreateDirectory(folderName);

                    try
                    {
                        File.WriteAllBytes(pathString, req.rawData);
                        _responses.Enqueue(new Response
                        {
                            from = req.from,
                            err = Protocol.Code.Error.None,
                            caller = Caller.FileDatabase,
                            agent = req.agent,
                            res = null
                        });
                    }
                    catch (IOException e)
                    {
                        Library.Tracer.Error(e.Message);
                    }
                }
                _writeWorker = null;
            }, _cts.Token);
        }
    }
}
