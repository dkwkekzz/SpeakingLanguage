using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic.Process
{
    internal static class Director
    {
        public static unsafe void Execute(EventManager eventManager, ref Service service)
        {
            ref readonly var colObj = ref service.colObj;
            var iter = eventManager.GetControllerEnumerator();
            while (iter.MoveNext())
            {
                var controller = iter.Current;
                var selectedSubjectHandle = new slObjectHandle( controller.objectHandleValue );
                var pSubject = colObj.Find(selectedSubjectHandle);
                if (null == pSubject)
                {
                    Library.Tracer.Error($"[Process::Director] no found subject: {selectedSubjectHandle.ToString()}");
                    continue;
                }

                var controlState = slObjectHelper.GetControlState(pSubject);
                if (null == controlState)
                {
                    Library.Tracer.Error($"[Process::Director] no found control: {selectedSubjectHandle.ToString()}");
                    continue;
                }

                switch (controller.type)
                {
                    case ControlType.Keyboard:
                        var key = (ConsoleKey)controller.key;
                        switch (key)
                        {
                            case ConsoleKey.LeftArrow:
                                controlState->direction &= controller.value;
                                break;
                            case ConsoleKey.RightArrow:
                                controlState->direction &= (controller.value << 2);
                                break;
                            case ConsoleKey.UpArrow:
                                controlState->direction &= (controller.value << 4);
                                break;
                            case ConsoleKey.DownArrow:
                                controlState->direction &= (controller.value << 8);
                                break;
                            case 0: // ctrl
                                controlState->keyFire &= (controller.value);
                                break;
                            case (ConsoleKey)1: // alt
                                controlState->keyFire &= (controller.value << 1);
                                break;
                            case (ConsoleKey)2: // shift
                                controlState->keyFire &= (controller.value << 2);
                                break;
                            case ConsoleKey.A: 
                                controlState->keyFire &= (controller.value << 3);
                                break;
                            case ConsoleKey.S:
                                controlState->keyFire &= (controller.value << 4);
                                break;
                            case ConsoleKey.D:
                                controlState->keyFire &= (controller.value << 5);
                                break;
                            case ConsoleKey.W:
                                controlState->keyFire &= (controller.value << 6);
                                break;
                        }
                        break;
                    case ControlType.Touch:
                        controlState->touchTarget = controller.key;
                        controlState->touchFire = controller.value;
                        break;
                }
            }
        }
    }
}
