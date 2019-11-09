using System;
using System.Collections.Generic;
using System.Reflection;

namespace SpeakingLanguage.Logic
{
    internal abstract unsafe class SelfAction : IAction
    {
        public sealed class Comparer : IComparer<SelfAction>
        {
            public int Compare(SelfAction x, SelfAction y)
            {
                var xh = (long)x._condTypeHandle;
                var yh = (long)y._condTypeHandle;
                if (xh == yh)
                    return 0;
                return xh < yh ? -1 : 1;
            }
        }
        
        private IntPtr _condTypeHandle;
        private float _rate = 0f;
        private float _accumed = 0f;

        public ActionType Type => ActionType.Self;

        public virtual void Initialize(MethodInfo mth, Type[] condTypes, Type[] targetTypes)
        {
            _condTypeHandle = condTypes[0].TypeHandle.Value;

            var frameAttr = Attribute.GetCustomAttribute(mth, typeof(FrameAttribute)) as FrameAttribute;
            if (null != frameAttr)
            {
                _rate = frameAttr.Per / frameAttr.Frame;
            }
        }

        protected bool Vaild(ref FrameManager ctx)
        {
            if (_rate == 0)
                return true;

            _accumed += ctx.Delta;
            if (_rate > _accumed)
                return false;

            _accumed -= _rate;
            return true;
        }

        public abstract void Invoke(StreamingContext* ctx);
    }

    internal unsafe sealed class SelfAction<T> : SelfAction where T : unmanaged
    {
        private delegate void SelfDel(StreamingContext* ctx, T* src);
        private SelfDel _del;
        
        public override void Initialize(MethodInfo mth, Type[] condTypes, Type[] targetTypes)
        {
            _del = (SelfDel)mth.CreateDelegate(typeof(SelfDel));
            base.Initialize(mth, condTypes, null);
        }

        public override void Invoke(StreamingContext* ctx)
        {
            if (!base.Vaild(ref ctx->frameManager))
                return;

            _del(ctx, (T*)ctx->src1);
        }
    }
}
