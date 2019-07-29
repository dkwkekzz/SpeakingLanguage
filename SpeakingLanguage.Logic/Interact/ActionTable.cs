using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Interact
{
    internal class ActionTable
    {
        private readonly static Type TInteractionAttribute = typeof(InteractionAttribute);

        private readonly IReadOnlyDictionary<ActionType, IAction> _dicActions;

        public ActionTable()
        {
            _dicActions = _collectActions();
        }

        public bool TryGetValue(ActionType t, out IAction action) => _dicActions.TryGetValue(t, out action);

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

                        Define.Relation relation = Define.Relation.None;
                        switch (paramCount)
                        {
                            case 2:
                                relation = Define.Relation.Self;
                                break;
                            case 3:
                                relation = Define.Relation.Simple;
                                break;
                        }

                        if (relation == Define.Relation.None)
                            throw new ArgumentException("relation can't be none: parameter count is zero.");

                        Type actionType = typeof(SimpleAction<>);
                        Type[] typeArgs = { interAttr.SrcType };
                        Type constructed = actionType.MakeGenericType(typeArgs);

                        var action = Activator.CreateInstance(constructed) as IAction;
                        action.Take(mth);

                        dicActions.Add(new ActionType { type = interAttr.SrcType, relation = relation }, action);
                    }
                }
            }

            return dicActions;
        }
    }
}
