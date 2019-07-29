﻿using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public struct StartInfo
    {
        public long startTick;
        public int startFrame;

        public int max_byte_terminal;
        public int max_byte_world_service;
        public int max_byte_interact_service;

        public int default_count_cell;
        public int default_count_cellnode;
        public int max_count_observer;


        public int max_byte_entitymanager_heap;
        public int max_byte_command_heap;
        public int max_count_command_type;
        public int max_count_entity_type;
        public int max_count_event_type;
        public int max_count_state_buffer;
    }
}
