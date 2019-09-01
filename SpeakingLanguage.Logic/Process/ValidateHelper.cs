using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Process
{
    internal static class ValidateHelper
    {
        public static unsafe bool ValidateInteract(Default* lstate, Default* rstate)
        {
            var diffX = Math.Abs(lstate->position.x - rstate->position.x);
            var diffY = Math.Abs(lstate->position.y - rstate->position.y);
            var distance = lstate->detection.radius + rstate->detection.radius;
            return diffX * diffX + diffY * diffY >= distance * distance;
        }
    }
}
