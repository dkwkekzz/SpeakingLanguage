using System;
using System.Collections.Generic;
using System.Reflection;

namespace SpeakingLanguage.Logic.Interact
{
    internal struct ActionType : IEquatable<ActionType>
    {
        public Type type;
        public Define.Relation relation;

        public bool Equals(ActionType other)
        {
            return other.type == type && other.relation == relation;
        }

        public override int GetHashCode()
        {
            return type.GetHashCode() ^ (int)relation;
        }
    }
}
