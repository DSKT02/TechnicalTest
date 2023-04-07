using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Ammunition
{
    protected override void ShootBehaviour(Vector3 direction)
    {
        targetRigidbody.velocity = direction.normalized * baseSpeed;
        targetRigidbody.transform.LookAt(targetRigidbody.transform.position + direction.normalized);
    }
}
