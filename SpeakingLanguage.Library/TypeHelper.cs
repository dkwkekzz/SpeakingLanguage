using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpeakingLanguage.Library
{
    public static class TypeHelper
    {
        public static bool HasAttribute(Type classType, Type attrType) => classType.GetCustomAttributes(attrType, true).Length > 0;
        public static bool HasAttribute(Type classType, Type attrType, Predicate<Attribute> condition)
        {
            foreach (var attr in classType.GetCustomAttributes(attrType, true))
            {
                if (condition(attr as Attribute))
                    return true;
            }

            return false;
        }

        public static bool HasInterface(Type classType, string iName) => classType.GetInterface(iName) != null;
    }
}
