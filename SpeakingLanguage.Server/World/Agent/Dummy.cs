using System;
using System.Collections.Generic;
using SpeakingLanguage.Library;
using SpeakingLanguage.Logic;

namespace SpeakingLanguage.Server
{
    internal sealed class Dummy : IAgent
    {
        public int Id => throw new NotImplementedException();
        public slObjectHandle SubjectHandle => throw new NotImplementedException();
    }
}
