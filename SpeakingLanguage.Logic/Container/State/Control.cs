using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public struct Control : IState
    {
        public int direction;
        public int keyFire;
        public int touchTarget;
        public int touchFire;
    }
}
