using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private float deadDelay = 0.5f;

    [SerializeField]
    private List<UnityEngine.Object> deactiveOnDeath;

    private float currentHealth;
    public float CurrentHealth
    {
        get => currentHealth;
        private set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            OnHealthChange?.Invoke(currentHealth / (maxHealth == 0 ? Mathf.Epsilon : maxHealth));
            if (currentHealth == 0) { OnHealthReachZero?.Invoke(); StartCoroutine(C_DelayedDead()); }
        }
    }

    public Action<float> OnHealthChange { get; set; }
    public Action OnHealthReachZero { get; set; }
    public Action OnDead { get; set; }
    public float MaxHealth { get => maxHealth; }

    private IEnumerator Start()
    {
        yield return null;
        CurrentHealth = maxHealth;
    }

    public void ChangeHealth(float amount)
    {
        if (CurrentHealth <= 0) return;
        CurrentHealth += amount;
    }

    private IEnumerator C_DelayedDead()
    {
        foreach (var item in deactiveOnDeath)
        {
            Destroy(item);
        }
        yield return new WaitForSeconds(deadDelay);
        OnDead?.Invoke();
    }
}
