using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    public class Config
    {
        public static bool debug { get; } = ("debug").ParseConfigOrDefault(true);
    }
}
