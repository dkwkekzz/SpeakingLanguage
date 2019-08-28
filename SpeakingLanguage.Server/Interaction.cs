using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal static class Interaction
    {
        [Logic.Subject(typeof(Logic.Position))]
        static unsafe void UpdateCollider(ref Logic.slActionContext ctx)
        {
            var subjectHandle = ctx.subject.Handle;
            var pos = ctx.subject.Get<Logic.Position>();

            WorldManager.Locator.Colliders.Update(subjectHandle, *pos);
        }
    }
}
