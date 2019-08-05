using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public interface IState<TResult>
    {
        TResult Get(int type);
        TResult Get(Define.Controller type);
    }
}
