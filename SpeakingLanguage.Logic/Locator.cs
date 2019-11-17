using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public sealed class Locator : Library.SingletonLazy<Locator>
    {
        public PropertyTable PropertyTable { get; } = new PropertyTable();
        public ArchetypeCollection ArchetypeCollection { get; } = new ArchetypeCollection();
        //public MessageQueue PropertyMessageQueue { get; } = new MessageQueue();
        internal Graph Graph { get; } = new Graph();
        internal LawMediator LawMediator { get; } = new LawMediator();
        internal LawCollection LawCollection { get; } = new LawCollection();
    }
}
