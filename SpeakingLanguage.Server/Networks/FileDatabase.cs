using LiteNetLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SpeakingLanguage.Server.Networks
{
    public enum DataType
    {
        Empty = 0,
        User,
        Object,
    }

    internal sealed class FileDatabase : IDatabase
    {
        private struct ReadRequest
        {
            public ISerializable agent;
            public string fileKey;
        }

        private struct ReadResponse
        {
            public ISerializable agent;
            public byte[] rawData;
        }

        private struct WriteRequest
        {
            public ISerializable agent;
            public string fileKey;
        }

        private struct WriteResponse
        {
            public ISerializable agent;
        }

        private readonly ConcurrentQueue<ReadRequest> _readRequests = new ConcurrentQueue<ReadRequest>();
        private readonly ConcurrentQueue<ReadResponse> _readResponses = new ConcurrentQueue<ReadResponse>();
        private readonly ConcurrentQueue<WriteRequest> _writeRequests = new ConcurrentQueue<WriteRequest>();
        private readonly ConcurrentQueue<WriteResponse> _writeResponses = new ConcurrentQueue<WriteResponse>();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly byte[] _buffer = new byte[4096];
        
        private Task _readWorker;
        private Task _writeWorker;

        public FileDatabase()
        {
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        public void FlushResponse()
        {
            if (_readRequests.Count > 0) _readAsync();
            if (_writeRequests.Count > 0) _writeAsync();

            while (_readResponses.Count > 0)
            {
                if (!_readResponses.TryDequeue(out ReadResponse res)) continue;

                var reader = new Library.Reader(res.rawData, res.rawData == null ? 0 : res.rawData.Length);
                res.agent.DeserializeInfo(ref reader);
            }

            while (_writeResponses.Count > 0)
            {
                if (!_writeResponses.TryDequeue(out WriteResponse res)) continue;

                // nothing...
            }
        }

        public void RequestReadUser(User agent, string id, string pswd)
        {
            _readRequests.Enqueue(new ReadRequest { agent = agent, fileKey = $"user_{id}_{pswd}" });
            _readAsync();
        }

        public void RequestWriteUser(User agent, string fileKey)
        {
            _writeRequests.Enqueue(new WriteRequest { agent = agent, fileKey = fileKey });
            _writeAsync();
        }

        //public void RequestReadObject(User agent, long objUid)
        //{
        //    _readRequests.Enqueue(new ReadRequest { agent = agent, fileKey = $"obj_{objUid.ToString()}" });
        //    _readAsync();
        //}

        //public void RequestWriteObject(User agent, long objUid)
        //{
        //    _writeRequests.Enqueue(new WriteRequest { agent = agent, fileKey = $"obj_{objUid.ToString()}" });
        //    _writeAsync();
        //}

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
                        if (!File.Exists(pathString))
                        {
                            _readResponses.Enqueue(new ReadResponse { agent = req.agent, rawData = null });
                            continue;
                        }

                        var rawData = File.ReadAllBytes(pathString);
                        _readResponses.Enqueue(new ReadResponse { agent = req.agent, rawData = rawData });
                    }
                    catch (IOException e)
                    {
                        Library.Tracer.Error(e.Message);
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
                        using (var stream = File.Create(pathString))
                        {
                            var writer = new Library.Writer(_buffer, 0);
                            req.agent.SerializeInfo(ref writer);
                            stream.Write(_buffer, 0, writer.Offset);
                        }
                    }
                    catch (IOException e)
                    {
                        Library.Tracer.Error(e.Message);
                    }
                }
                _writeWorker = null;
            }, _cts.Token);
        }

        //private void _doWorkAsync()
        //{
        //    if (_worker != null) return;
        //
        //    _worker = Task.Factory.StartNew((object obj) =>
        //    {
        //        CancellationToken token = (CancellationToken)obj;
        //        while (!token.IsCancellationRequested && _userRequests.Count > 0)
        //        {
        //            var res = _userRequests.Dequeue();
        //
        //            string folderName = @"FileDatabase";
        //            string fileKey = $"user_{res.id.ToString()}";
        //            string pathString = Path.Combine(folderName, fileKey);
        //
        //            Directory.CreateDirectory(folderName);
        //
        //            try
        //            {
        //                if (File.Exists(pathString))
        //                {
        //                    var rawData = File.ReadAllBytes(pathString);
        //                    _userResponses.Enqueue(new UserResponse { request = res, rawData = rawData });
        //                }
        //                else
        //                {
        //                    _userResponses.Enqueue(new UserResponse { request = res, rawData = null });
        //                }
        //            }
        //            catch (IOException e)
        //            {
        //                Library.Tracer.Error(e.Message);
        //            }
        //        }
        //
        //        while (!token.IsCancellationRequested && _objRequests.Count > 0)
        //        {
        //            var res = _objRequests.Dequeue();
        //
        //            string folderName = @"FileDatabase";
        //            string fileKey = $"obj_{res.handleValue.ToString()}";
        //            string pathString = Path.Combine(folderName, fileKey);
        //
        //            Directory.CreateDirectory(folderName);
        //
        //            try
        //            {
        //                if (File.Exists(pathString))
        //                {
        //                    var rawData = File.ReadAllBytes(pathString);
        //                    _objResponses.Enqueue(new ObjectResponse { request = res, rawData = rawData });
        //                }
        //                else
        //                {
        //                    _objResponses.Enqueue(new ObjectResponse { request = res, rawData = null });
        //                }
        //            }
        //            catch (IOException e)
        //            {
        //                Library.Tracer.Error(e.Message);
        //            }
        //        }
        //    }, _cts.Token);
        //}
    }
}
