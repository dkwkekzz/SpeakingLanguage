using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Logic.TestSpace
{
    struct Controller
    {
        public int value1;
        public int value2;
        public int value3;
        public int value4;
    }
    
    class Health
    {

    }

    [Logic.ActionProviderAttribute]
    internal static class TestSystem
    {
        [Logic.SubjectAttribute(typeof(Logic.Default))]
        static void Test1(ref Logic.slActionContext ctx)
        {
            var subjectHandle = ctx.subject.Handle;
            var logicState = ctx.subject.GetRef<Logic.Default>();
            UpdateProjectile(ref logicState, ctx.delta);
        }

        static unsafe void UpdateProjectile(ref Default logicState, float time)
        {
            logicState.position.x += (int)(logicState.detection.radius * time);
            logicState.position.y += (int)(logicState.detection.radius * time);
            logicState.position.z += (int)(logicState.detection.radius * time);
        }
    }

    class TestSpace
    {
    }
}
