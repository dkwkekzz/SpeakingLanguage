using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal sealed class Locator : Library.SingletonLazy<Locator>
    {
        public PropertyTable PropertyTable { get; } = new PropertyTable();
        public ArchetypeCollection ArchetypeCollection { get; } = new ArchetypeCollection();
        public Graph Graph { get; } = new Graph();
        public ControlTower ControlTower { get; } = new ControlTower();
        public LawMediator LawMediator { get; } = new LawMediator();
        public LawCollection LawCollection { get; } = new LawCollection();
    }
}
