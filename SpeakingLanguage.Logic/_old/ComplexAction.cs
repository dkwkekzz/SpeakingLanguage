using System;
using System.Collections.Generic;
using System.Reflection;

namespace SpeakingLanguage.Logic
{
    internal static class ActionHelper
    {
        public static IAction CreateNewAction(SubjectAttribute subAttr, MethodInfo mth)
        {
            int argsCount = subAttr.SrcList.Length;

            var TTargetAttribute = typeof(TargetAttribute);
            var targetAttr = Attribute.GetCustomAttribute(mth, TTargetAttribute) as TargetAttribute;
            if (targetAttr != null)
            {
                argsCount += targetAttr.SrcList.Length;
            }

            var typeArgs = new Type[argsCount];
            var condLength = subAttr.SrcList.Length;
            for (int i = 0; i != condLength; i++)
                typeArgs[i] = subAttr.SrcList[i];
            
            if (targetAttr != null)
            {
                var targetLength = targetAttr.SrcList.Length;
                for (int i = 0; i != targetLength; i++)
                    typeArgs[condLength + i] = targetAttr.SrcList[i];
            }
            
            Type actionType = null;
            if (argsCount == 1)
            {
                actionType = typeof(SelfAction<>);
            }
            else if (argsCount == 2 && condLength == 1)
            {
                actionType = typeof(SimpleAction<,>);
            }

            var constructed = actionType.MakeGenericType(typeArgs);
            var action = Activator.CreateInstance(constructed) as IAction;
            action.Initialize(mth, subAttr.SrcList, targetAttr.SrcList);

            return action;
        }
    }

    internal unsafe sealed class SimpleAction<T1, T2> : InteractAction, IAction 
        where T1 : unmanaged
        where T2 : unmanaged
    {
        private delegate void Del(InteractContext* ctx, T1* src1, T2* src2);
        private Del _del;
        private IntPtr _condTypeHandle;
        private IntPtr _targetTypeHandle;

        public ActionType Type => ActionType.Interact;

        public void Initialize(MethodInfo mth, Type[] condTypes, Type[] targetTypes)
        {
            _del = (Del)mth.CreateDelegate(typeof(Del));
            _condTypeHandle = condTypes[0].TypeHandle.Value;
            _targetTypeHandle = targetTypes[0].TypeHandle.Value;

            base.Initialize(mth);
        }

        public void Invoke(InteractContext* ctx)
        {
            if (!Vaild(ctx))
                return;

            _del(ctx, (T*)ctx->src);
        }
    }

    internal unsafe sealed class ComplexAction<T1, T2, T3> : InteractAction, IAction
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        private delegate void Del(InteractContext* ctx, T1* src1, T2* src2, T3* src3);
        private Del _del;
        private IntPtr[] _condTypeHandles;
        private IntPtr[] _targetTypeHandles;

        public ActionType Type => ActionType.Interact;

        public void Initialize(MethodInfo mth, Type[] condTypes, Type[] targetTypes)
        {
            _del = (Del)mth.CreateDelegate(typeof(Del));

            var condLength = condTypes.Length;
            _condTypeHandles = new IntPtr[condLength];
            for (int i = 0; i != condLength; i++)
                _condTypeHandles[i] = condTypes[i].TypeHandle.Value;

            var targetLength = targetTypes.Length;
            _targetTypeHandles = new IntPtr[targetLength];
            for (int i = 0; i != targetLength; i++)
                _targetTypeHandles[i] = targetTypes[i].TypeHandle.Value;
            
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
