#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace Terresquall.Devices {
    [Preserve]
    [InputControlLayout(displayName = "Virtual Joystick")]
    public class VirtualJoystick : Joystick {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() {
            InputSystem.RegisterLayout<VirtualJoystick>();
        }
    }
}
#endif
