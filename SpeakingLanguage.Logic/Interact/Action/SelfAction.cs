using System;
using System.Collections.Generic;
using System.Reflection;

namespace SpeakingLanguage.Logic.Interact
{
    internal unsafe sealed class SelfAction<T> : InteractAction, IAction where T : unmanaged
    {
        private delegate void SelfDel(IPublicContext ctx, T* pa);
        private SelfDel _del;

        public void Take(MethodInfo mth)
        {
            _del = (SelfDel)mth.CreateDelegate(typeof(SelfDel));

            base.Initialize(mth);
        }

        public void Invoke(ref CallContext ctx)
        {
            if (!Vaild(ref ctx))
                return;

            _del((IPublicContext)ctx.args[0], (T*)ctx.src);
        }
    }
}
