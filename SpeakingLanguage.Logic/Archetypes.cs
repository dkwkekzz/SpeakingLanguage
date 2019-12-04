using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public sealed class AccountArchetype : ArchetypeHolder<AccountArchetype>
    {
        protected override void OnConstruct(out PropertySet property)
        {
            property = new PropertySet(typeof(Property.Viewier), typeof(Property.Position));
        }
    }

    public sealed class ObserverArchetype : ArchetypeHolder<ObserverArchetype>
    {
        protected override void OnConstruct(out PropertySet property)
        {
            property = new PropertySet(typeof(Property.Position), typeof(Property.Controller));
        }
    }

}
