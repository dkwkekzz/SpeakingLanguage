using System;
using System.Collections.Generic;
using System.Threading;

namespace SpeakingLanguage.Logic.Process
{
    internal interface IJobContext
    {
        SyncHandle SyncHandle { get; }
        JobPartitioner JobPartitioner { get; }
        CancellationToken Token { get; }
    }
}
