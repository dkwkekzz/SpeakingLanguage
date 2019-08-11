using System;
using System.Collections.Generic;
using System.Reflection;

namespace SpeakingLanguage.Logic
{
    internal unsafe sealed class SelfAction<T> : InteractAction, IAction where T : unmanaged
    {
        private delegate void SelfDel(InteractContext* ctx, T* pa);
        private SelfDel _del;

        public void Take(MethodInfo mth)
        {
            _del = (SelfDel)mth.CreateDelegate(typeof(SelfDel));

            base.Initialize(mth);
        }

        public void Invoke(InteractContext* ctx)
        {
            if (!Vaild(ctx))
                return;

            _del(ctx, (T*)ctx->src);
        }
    }
}
