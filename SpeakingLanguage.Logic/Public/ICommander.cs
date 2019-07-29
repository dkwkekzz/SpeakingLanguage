using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public interface ICommander<TCommand>
    {
        TCommand GetCommand(Type type);
    }
}
