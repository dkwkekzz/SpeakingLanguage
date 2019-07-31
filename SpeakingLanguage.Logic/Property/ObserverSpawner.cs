﻿using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Property
{
    public unsafe struct ObserverSpawner
    {
        public int count;
        public int blockSize;
        public long tick;
        public ObserverInfo* info;
    }
}
