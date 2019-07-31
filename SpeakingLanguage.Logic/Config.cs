using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    internal class Config
    {   // 없애는중...
        public static long START_TICK { get; private set; } = 0;
        public static int START_FRAME { get; private set; } = 0;

        public static int MAX_BYTE_TERMINAL { get; private set; } = 2048;
        public static int MAX_BYTE_ENTITYMANAGER_HEAP { get; private set; } = 1024;
        public static int MAX_BYTE_COMMAND_HEAP { get; private set; } = 64;
        public static int MAX_COUNT_COMMAND_TYPE { get; private set; } = 16;
        public static int MAX_COUNT_ENTITY_TYPE { get; private set; } = 16;
        public static int MAX_COUNT_EVENT_TYPE { get; private set; } = 16;
        public static int MAX_COUNT_STATE_BUFFER { get; private set; } = 16;
        public static int MAX_COUNT_CELL { get; private set; } = 256;
        public static int MAX_COUNT_OBSERVER { get; private set; } = 256;

        public static void Construct(ref StartInfo info)
        {
            START_TICK = info.startTick;
            START_FRAME = info.startFrame;

            MAX_BYTE_TERMINAL = info.max_byte_terminal;
            MAX_BYTE_ENTITYMANAGER_HEAP = info.max_byte_entitymanager_heap;
            MAX_BYTE_COMMAND_HEAP = info.max_byte_command_heap;
            MAX_COUNT_COMMAND_TYPE = info.max_count_command_type;
            MAX_COUNT_ENTITY_TYPE = info.max_count_entity_type;
            MAX_COUNT_EVENT_TYPE = info.max_count_event_type;
            MAX_COUNT_STATE_BUFFER = info.max_count_state_buffer;
            MAX_COUNT_CELL = info.default_count_cell;
            MAX_COUNT_OBSERVER = info.default_count_observer;
        }
    }
}
