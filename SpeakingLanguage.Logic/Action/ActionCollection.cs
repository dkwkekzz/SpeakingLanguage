using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal class ActionCollection
    {
        private struct ActionType : IEquatable<ActionType>
        {
            public IntPtr typeHandle;
            public Define.Relation relation;

            public bool Equals(ActionType other)
            {
                return other.typeHandle == typeHandle && other.relation == relation;
            }

            public override int GetHashCode()
            {
                return typeHandle.ToInt32() ^ (int)relation;
            }
        }
        
        private readonly IReadOnlyList<SelfAction> _lstSelfActions;
        private readonly IReadOnlyList<IAction> _lstActions;

        public ActionCollection()
        {
            var lstSelfActions = new List<SelfAction>();
            var lstActions = new List<IAction>();

            var TConditionAttribute = typeof(ConditionAttribute);
            var types = Library.AssemblyHelper.CollectType(null);
            foreach (var type in types)
            {
                var mths = type.GetMethods();
                for (int i = 0; i != mths.Length; i++)
                {
                    var mth = mths[i];

                    var condAttr = Attribute.GetCustomAttribute(mth, TConditionAttribute) as ConditionAttribute;
                    if (condAttr != null)
                    {
                        var action = ActionHelper.CreateNewAction(condAttr, mth);
                        if (action.Type == Logic.ActionType.Self)
                        {
                            lstSelfActions.Add(action as SelfAction);
                        }
                        else
                        {
                            lstActions.Add(action);
                        }
                    }
                }
            }

            lstSelfActions.Sort(new SelfAction.Comparer());

            _lstSelfActions = lstSelfActions;
            _lstActions = lstActions;
        }

        public void GetSelfEnumerator()
        {

        }
        
    }
}
