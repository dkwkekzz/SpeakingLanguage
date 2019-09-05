using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Server
{
    internal static class Interaction
    {
        [Logic.SubjectAttribute(typeof(Logic.Default))]
        static void UpdateCollider(ref Logic.slActionContext ctx)
        {
            var subjectHandle = ctx.subject.Handle;
            var logicState = ctx.subject.Get<Logic.Default>();
            WorldManager.Instance.Colliders.Update(subjectHandle.value, ref logicState.position, ref logicState.detection);
        }
    }
}
