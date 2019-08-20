using System;
using System.Collections.Generic;
using System.Reflection;

namespace SpeakingLanguage.Logic.Interact
{
    internal unsafe sealed class SimpleAction<T> : InteractAction, IAction where T : unmanaged
    {
        private delegate void SimpleDel(IPublicContext ctx, T* pa, IEntityManager pother);
        private SimpleDel _del;

        public void Take(MethodInfo mth)
        {
            _del = (SimpleDel)mth.CreateDelegate(typeof(SimpleDel));

            base.Initialize(mth);
        }

        public void Invoke(ref CallContext ctx)
        {
            if (!Vaild(ref ctx))
                return;

            _del((IPublicContext)ctx.args[0], (T*)ctx.src, (IEntityManager)ctx.args[1]);
        }
    }
}
