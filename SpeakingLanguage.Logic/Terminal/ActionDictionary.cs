using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal class ActionDictionary
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

        private readonly static Type TInteractionAttribute = typeof(InteractionAttribute);

        private readonly IReadOnlyDictionary<ActionType, IAction> _dicActions;

        public ActionDictionary()
        {
            _dicActions = _collectActions();
        }

        public bool TryGetValue(IntPtr typeHandle, Define.Relation relation, out IAction action) 
            => _dicActions.TryGetValue(new ActionType { typeHandle = typeHandle, relation = relation }, out action);

        private Dictionary<ActionType, IAction> _collectActions()
        {
            var dicActions = new Dictionary<ActionType, IAction>();

            var types = Library.AssemblyHelper.CollectType(null);
            foreach (var type in types)
            {
                var mths = type.GetMethods();
                for (int i = 0; i != mths.Length; i++)
                {
                    var mth = mths[i];

                    var interAttr = Attribute.GetCustomAttribute(mth, TInteractionAttribute) as InteractionAttribute;
                    if (interAttr != null)
                    {
                        int paramCount = 0;
                        var paramInfos = mth.GetParameters();
                        for (int j = 0; j != paramInfos.Length; j++)
                        {
                            var paramType = paramInfos[j].ParameterType;
                            paramCount++;
                        }
                        
                        Type actionType = typeof(SelfAction<>);
                        Type[] typeArgs = { interAttr.SrcType };
                        Type constructed = actionType.MakeGenericType(typeArgs);

                        var action = Activator.CreateInstance(constructed) as IAction;
                        action.Take(mth);

                        dicActions.Add(new ActionType { typeHandle = interAttr.SrcType.TypeHandle.Value, relation = interAttr.Relation }, action);
                    }
                }
            }

            return dicActions;
        }
    }
}
