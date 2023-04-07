using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyStickInputController : MonoBehaviour
{
    [SerializeField]
    private MobileUIJoystick joyStickL, joyStickR;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private EntityAction<Vector2> aimAction, moveAction;

    [SerializeField]
    private EntityAction<bool> shootAction;

    [SerializeField]
    InputManager manager;

    private void Start()
    {
        joyStickL.OnJoystickMove += (_) => { if (manager.AllowInputs) moveAction.SetInput(_); };
        joyStickR.OnJoystickMove += (_) => { if (manager.AllowInputs) aimAction.SetInput(_ + new Vector2(target.position.x, target.position.z)); };
        joyStickR.OnJoystickMove += (_) => { if (manager.AllowInputs) shootAction.SetInput(_ != Vector2.zero); };
    }
}
