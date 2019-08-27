using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal static class Interaction
    {
        [Logic.Subject(typeof(Logic.Position))]
        static unsafe void ValidateScene(ref Logic.slActionContext ctx)
        {
            var subjectHandle = ctx.subject.Handle;


            var pos = ctx.subject.Get<Logic.Position>();

            // calculate scene handle

            // insert subject handle
        }
    }
}
