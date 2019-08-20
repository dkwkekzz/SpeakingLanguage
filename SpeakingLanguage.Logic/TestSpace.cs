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


    class Eventer
    {
        [Logic.SubjectAttribute(typeof(Controller))]
        public static unsafe void OnClientControl(ref Logic.ActionContext ctx)
        {
            var ctrl = ctx.subject.Get<Controller>()
            if (hasEvent())
            {
                ctrl->value1 = 5;
            }
        }
    }

    class Health
    {

    }

    class InteractSystem<TState>
    {
        private int stateHandle;

    }

    class TestSpace
    {
    }
}
