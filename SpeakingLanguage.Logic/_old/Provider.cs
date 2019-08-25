using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public unsafe struct StreamingContext
    {
        public FrameManager frameManager;
        public Streamer streamer;
        public void* src1;
        public void* src2;
        public void* src3;
        public void* src4;
        public void* src5;
    }

    public sealed class Provider : IDisposable
    {
        private readonly Library.umnMarshal _umnAllocator;
        private readonly slActionCollection _colAct;
        private readonly ObjectCollection _colObj;
        private readonly StreamingContext _ctx;

        public Provider(StartInfo info)
        {
            unsafe
            {
                _colAct = new slActionCollection();
                _colObj = new ObjectCollection(_umnAllocator.Alloc(info.max_byte_objectcollection));
                _ctx = new StreamingContext
                {
                    frameManager = new FrameManager(info.startFrame),
                    streamer = new Streamer(_umnAllocator.Calloc(info.max_byte_streamer)),
                };
            }
        }

        public void Dispose()
        {
            _umnAllocator.Dispose();
        }
        
        public static unsafe void ExecuteFrame()
        {
            fixed (StreamingContext* pCtx = &_ctx)
            {
                var objIter = _colObj.GetEnumerator();
                while (objIter.MoveNext())
                {
                    var pObj = objIter.Current;
                    var eIter = pObj->GetEnumerator();
                    while (eIter.MoveNext())
                    {
                        var eChk = eIter.Current;
                        // self or multi
                        // 개수만 얻어오게 해서 stack으로 쌓자.
                        // 맞는 엔터티가없으면, 다음액션으로 패스
                    }
                }

                // slo에서 entity를 뭔가 효과적인 방법으로 얻어와야...

                // lhs의 entity를 순회, 동시에 actioncollection도 순회한다.
                // actioncollection tag에 없으면 다음으로 스킵, 
                // 여기서 비교할때, pObj->current_e < action iterhandle, next 이런식으로
                // 

            }


        }
    }
}
