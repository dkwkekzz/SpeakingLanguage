using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Entity
{
    public unsafe struct Interaction
    {
        public Logic.slObject* dest;
        public int ofs_s_x;
        public int ofs_s_y;
        public int ofs_d_x;
        public int ofs_d_y;
        public int dis_x;
        public int dis_y;
    }
}
