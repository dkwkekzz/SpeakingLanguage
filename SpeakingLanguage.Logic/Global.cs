using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public struct Global
    {
        public static FrameManager FrameManager { get; private set; }
        public Streamer streamer;

        public static void Setup(ref StartInfo info)
        {
            FrameManager = new FrameManager(info.startFrame);

        }
    }
}
