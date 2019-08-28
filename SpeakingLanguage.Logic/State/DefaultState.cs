﻿using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public struct DefaultState : IState
    {
        public LifeCycle lifeCycle;
        public Spawner spawner;
        public Position position;
        public Detection detection;
    }
}
