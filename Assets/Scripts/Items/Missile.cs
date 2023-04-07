using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Ammunition
{
    [SerializeField]
    private float castInterval = 0.2f, detectionRadius = 2f, angularvelocity = 20;

    protected override void ShootBehaviour(Vector3 direction)
    {
        targetRigidbody.velocity = direction.normalized * baseSpeed;
        StopAllCoroutines();
        StartCoroutine(C_DetectEnemies());
    }

    private IEnumerator C_DetectEnemies()
    {
        while (true)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
            foreach (var item in hitColliders)
            {
                if (!item.TryGetComponent<CharacterAffiliation>(out var entity)) continue;

                if (entity.TeamID != Sender.TeamID)
                {
                    var tempVectorDirection = (entity.transform.position - transform.position);
                    tempVectorDirection = new Vector3(tempVectorDirection.x, 0, tempVectorDirection.z).normalized;

                    targetRigidbody.velocity = Vector3.MoveTowards(targetRigidbody.velocity.normalized, tempVectorDirection, angularvelocity * Time.deltaTime) * baseSpeed;

                    transform.LookAt(transform.position + targetRigidbody.velocity.normalized);
                    break;
                }
            }
            yield return new WaitForSeconds(castInterval);
        }

    }
}
