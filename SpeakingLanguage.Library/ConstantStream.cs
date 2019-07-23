using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SpeakingLanguage.Library
{
    public interface IDeserializer<TResult>
    {
        void Deserialize(byte[] streamBuffer, int count, out TResult result);
    }

    public interface ICapturing<TResult>
    {
        TResult Flush();
    }

    public class ConstantStream<TResult> : IEnumerable<TResult>, IDisposable
    {
        struct StreamingArgument
        {
            public long offset;
            public int size;
        }

        class CaptureProxy : ICapturing<TResult>
        {
            private IEnumerator<TResult> _streamItr;
            private StreamingArgument[] _args = new StreamingArgument[1];
            private int _readItr;
            private int _captureCount;

            public int Count => _captureCount;

            public CaptureProxy(IEnumerator<TResult> itr)
            {
                _streamItr = itr;
                _readItr = 0;
                _captureCount = 0;
            }
            
            public ICapturing<TResult> Put(long offset, int size)
            {
                if (_args.Length <= _captureCount)
                    throw new IndexOutOfRangeException("_args is full.");

                _args[_captureCount++] = new StreamingArgument { offset = offset, size = size };
                return this;
            }

            public bool TryTake(out StreamingArgument arg)
            {
                if (_readItr >= _captureCount)
                {
                    arg = default(StreamingArgument);
                    return false;
                }

                arg = _args[_readItr++];
                return true;
            }

            public TResult Flush()
            {
                if (!_streamItr.MoveNext())
                    return default(TResult);

                _readItr = 0;
                _captureCount = 0;
                return _streamItr.Current;
            }
        }

        private readonly string _binPath;
        private IDeserializer<TResult> _deserializer;
        private CaptureProxy _capturing;
        private Func<int, byte[]> _allocator;

        private bool _isRunning = false;
        private IEnumerator<TResult> _streamItr = null;

        public bool IsRunning => _isRunning;
        
        public ConstantStream(string binPath, IDeserializer<TResult> deserializer)
        {
            _binPath = binPath;
            _deserializer = deserializer;
        }

        public ConstantStream(string binPath, IDeserializer<TResult> deserializer, Func<int, byte[]> allocator)
        {
            _binPath = binPath;
            _deserializer = deserializer;
            _allocator = allocator;
        }

        public void Start()
        {
            if (_isRunning)
                return;
            
            _isRunning = true;
            _streamItr = GetEnumerator();
            _streamItr.MoveNext();
            _capturing = new CaptureProxy(_streamItr);
        }

        public void Stop()
        {
            Dispose();
        }

        public IEnumerator<TResult> GetEnumerator()
        {
            return _updateStream();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _updateStream();
        }

        private IEnumerator<TResult> _updateStream()
        {
            using (var fStream = new FileStream(_binPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                while (_isRunning)
                {
                    while (_capturing.Count == 0)
                        yield return default(TResult);

                    StreamingArgument arg;
                    if (_capturing.TryTake(out arg))
                    {
                        fStream.Seek(arg.offset, SeekOrigin.Begin);
                        
                        var streamBuffer = _allocator?.Invoke(arg.size) ?? new byte[arg.size];

                        var n = fStream.Read(streamBuffer, 0, arg.size);
                        _deserializer.Deserialize(streamBuffer, n, out TResult reader);
                        yield return reader;
                    }
                }
            }
        }

        public void Dispose()
        {
            _isRunning = false;
            _streamItr?.MoveNext();
            _streamItr?.Dispose();
        }
        
        public ICapturing<TResult> Capture(long offset, int size)
        {
            return _capturing.Put(offset, size);
        }
    }
}
