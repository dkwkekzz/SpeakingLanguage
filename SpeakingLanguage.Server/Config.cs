﻿using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    public class Config
    {
        public static bool debug { get; } = ("debug").ParseConfigOrDefault(true);
        public static int max_user_count { get; } = ("max_user_count").ParseConfigOrDefault(1000);
    }
}
