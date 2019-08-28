using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal static class Interaction
    {
        [Logic.Subject(typeof(Logic.DefaultState))]
        static unsafe void UpdateCollider(ref Logic.slActionContext ctx)
        {
            var subjectHandle = ctx.subject.Handle;
            var logicState = ctx.subject.Get<Logic.DefaultState>();

            WorldManager.Locator.Colliders.Update(subjectHandle, ref logicState->position, ref logicState->detection);
        }
    }
}
