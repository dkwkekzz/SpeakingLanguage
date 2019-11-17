using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal interface IAgent
    {
        int Id { get; }
        Logic.slObjectHandle SubjectHandle { get; }
    }
}
