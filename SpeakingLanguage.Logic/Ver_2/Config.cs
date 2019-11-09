using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public class Config
    {
        public static int default_frameRate { get; } = ("default_frameRate").ParseConfigOrDefault(60);
        public static int default_objectcount { get; } = ("default_objectcount").ParseConfigOrDefault(10);
        public static int default_interactcount { get; } = ("default_interactcount").ParseConfigOrDefault(5);
        public static int default_workercount { get; } = ("default_workercount").ParseConfigOrDefault(1);
        public static int default_jobchunklength { get; } = ("default_jobchunklength").ParseConfigOrDefault(16);
        public static int max_subjectstackbuffersize { get; } = ("max_subjectstackbuffersize").ParseConfigOrDefault(1024);

        public void Setting()
        {
        }
    }
}
