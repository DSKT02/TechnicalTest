using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : EntityAction<Vector2>
{
    [SerializeField]
    private Rigidbody target;

    [SerializeField]
    private float angularSpeed;

    private Vector3 aimDirection;
    private Vector3 lastAimDirection;

    private void Start()
    {
        lastAimDirection = Vector3.forward;
        AllowAction = false;
    }

    public override void SetInput(Vector2 _input)
    {
        base.SetInput(_input);
        Execute();
    }

    protected override void Action()
    {
        aimDirection = (new Vector3(CurrentInput.x, target.transform.position.y, CurrentInput.y) - target.transform.position).normalized;
        target.transform.LookAt(target.transform.position + aimDirection);
        if (aimDirection.x != 0 || aimDirection.z != 0)
        {
            lastAimDirection = aimDirection;
        }
    }

    void FixedUpdate()
    {
        if (AllowAction) return;

        aimDirection = new Vector3(target.velocity.x, 0, target.velocity.z).normalized;
        target.transform.LookAt(target.transform.position + lastAimDirection);
        if (aimDirection.x != 0 || aimDirection.z != 0)
        {
            lastAimDirection = Vector3.MoveTowards(lastAimDirection, aimDirection, angularSpeed * Time.fixedDeltaTime);
        }
    }
}
