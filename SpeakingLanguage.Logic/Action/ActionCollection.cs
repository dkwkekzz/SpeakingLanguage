﻿using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal class ActionCollection
    {
        private readonly List<slAction<StateSync>> _lstSelfAction;
        private readonly List<slAction<DupStateSync>> _lstComplexAction;

        public ActionCollection()
        {
            _lstSelfAction = new List<slAction<StateSync>>();
            _lstComplexAction = new List<slAction<DupStateSync>>();

            var TSubjectAttribute = typeof(SubjectAttribute);
            var TTargetAttribute = typeof(TargetAttribute);

            var types = Library.AssemblyHelper.CollectType(null);
            foreach (var type in types)
            {
                var mths = type.GetMethods();
                for (int i = 0; i != mths.Length; i++)
                {
                    var mth = mths[i];

                    var subAttr = Attribute.GetCustomAttribute(mth, TSubjectAttribute) as SubjectAttribute;
                    if (subAttr != null)
                    {
                        List<int> subTypes = subAttr.SrcList;
                        List<int> targetTypes = null;
                        var targetAttr = Attribute.GetCustomAttribute(mth, TTargetAttribute) as TargetAttribute;
                        if (targetAttr != null)
                        {
                            targetTypes = targetAttr.SrcList;
                        }
                        
                        if (null == targetAttr || 0 == targetTypes.Count)
                        {
                            _lstSelfAction.Add(new slAction<StateSync>(mth, 
                                StateSync.Create(subTypes.GetEnumerator())));
                        }
                        else
                        {
                            _lstComplexAction.Add(new slAction<DupStateSync>(mth, 
                                DupStateSync.Create(subTypes.GetEnumerator(), targetTypes.GetEnumerator())));
                        }
                    }
                }
            }

            _lstSelfAction.Sort();
            _lstComplexAction.Sort();
        }

        public void InvokeSelf(ref ActionContext actionCtx, ref StateSync sync)
        {
            var iter = _lstSelfAction.GetEnumerator();
            while (iter.MoveNext())
            {
                var action = iter.Current;
                var actKey = action.Key;
                if (actKey.CompareTo(sync) > 0)
                    return;

                if (sync.Contains(ref actKey))
                    action.Invoke(ref actionCtx);
            }
        }

        public unsafe void InvokeComplex(ref ActionContext actionCtx, ref DupStateSync sync)
        {
            var iter = _lstComplexAction.GetEnumerator();
            while (iter.MoveNext())
            {
                var action = iter.Current;
                var actDupKey = action.Key;
                if (actDupKey.subject.CompareTo(sync.subject) > 0 &&
                    actDupKey.target.CompareTo(sync.target) > 0)
                    return;

                if (sync.subject.Contains(ref actDupKey.subject) &&
                    sync.target.Contains(ref actDupKey.target))
                    action.Invoke(ref actionCtx);
            }
        }
    }
}
