using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    [SerializeField]
    private Animator animationController;

    [SerializeField]
    private Rigidbody targetRigidbody;

    [SerializeField]
    private EntityAction<bool> shootAction;

    private void Start()
    {
        shootAction.ActionEvent += (_) => { animationController.SetBool("Shooting", _); };
    }

    private void LateUpdate()
    {
        animationController.SetBool("Moving", new Vector2(targetRigidbody.velocity.x, targetRigidbody.velocity.z).magnitude > 0.05f);
    }
}
