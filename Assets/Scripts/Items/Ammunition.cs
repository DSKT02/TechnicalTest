using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ammunition : MonoBehaviour, I_Pooleable<Ammunition>
{
    bool I_Pooleable<Ammunition>.Active { get; set; }

    [SerializeField]
    protected Rigidbody targetRigidbody;

    [SerializeField]
    protected MeshRenderer projectileRender;

    [SerializeField]
    private float lifeDuration;

    [SerializeField]
    private float delayToDisable;

    [SerializeField]
    protected float damage;

    [SerializeField]
    protected float baseSpeed;

    private CharacterAffiliation sender;

    public Action OnImpact { get; set; }
    public CharacterAffiliation Sender { get => sender; }

    protected abstract void ShootBehaviour(Vector3 direction);

    public void Shoot(CharacterAffiliation _sender, Vector3 direction)
    {
        sender = _sender;
        projectileRender.enabled = true;
        ShootBehaviour(direction);
        StartCoroutine(C_LifeCicle());
    }

    void I_Pooleable<Ammunition>.ClearState()
    {
        sender = null;
        targetRigidbody.velocity = Vector3.zero;
    }

    private void OnDisable()
    {
        (this as I_Pooleable<Ammunition>).Active = false;
        (this as I_Pooleable<Ammunition>).ClearState();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<CharacterAffiliation>(out var reciver))
        {
            StopAllCoroutines();
            StartCoroutine(C_DelayedDead());
            return;
        }

        if (reciver.CharacterID == sender.CharacterID) return;

        if (reciver.TeamID == sender.TeamID) return;

        if (!other.TryGetComponent<Health>(out var reciverHealth)) return;

        reciverHealth.ChangeHealth(-damage);

        StopAllCoroutines();
        StartCoroutine(C_DelayedDead());
    }

    private IEnumerator C_LifeCicle()
    {
        yield return new WaitForSeconds(lifeDuration);
        StartCoroutine(C_DelayedDead());
    }

    private IEnumerator C_DelayedDead()
    {
        OnImpact?.Invoke();
        projectileRender.enabled = false;
        targetRigidbody.velocity = Vector3.zero;
        yield return new WaitForSeconds(delayToDisable);
        gameObject.SetActive(false);
    }

    public string GetFromPoolCondition()
    {
        return "";
    }
}

