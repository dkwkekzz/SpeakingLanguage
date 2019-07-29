using System;
using System.Collections.Generic;
using System.Reflection;

namespace SpeakingLanguage.Logic.Interact
{
    internal abstract class InteractAction
    {
        private float _rate = 0f;
        private float _accumed = 0f;

        protected void Initialize(MethodInfo mth)
        {
            var frameAttr = Attribute.GetCustomAttribute(mth, typeof(FrameAttribute)) as FrameAttribute;
            if (null != frameAttr)
            {
                _rate = frameAttr.Per / frameAttr.Frame;
            }
        }
        
        protected bool Vaild(ref CallContext ctx)
        {
            if (_rate == 0)
                return true;

            _accumed += ctx.delta;
            if (_rate > _accumed)
                return false;

            _accumed -= _rate;
            return true;
        }
    }
}
