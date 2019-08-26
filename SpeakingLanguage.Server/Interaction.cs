using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal static class Interaction
    {
        [Logic.Subject(typeof(Logic.Position))]
        static unsafe void SelectScene(ref Logic.slActionContext ctx)
        {
            var pos = ctx.subject.Get<Logic.Position>();

            // calculate scene handle

            // insert subject handle
        }
    }
}
