using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolectableHealthPack : MonoBehaviour, I_Recolectable
{
    [SerializeField]
    private float healthUnit;

    [field: SerializeField] public int Amount { get; set; }

    public void Recolect(Inventory inventory)
    {
        if (!inventory.TryGetComponent<Health>(out var entityHealth)) return;
        if (entityHealth.CurrentHealth == entityHealth.MaxHealth) return;
        entityHealth.ChangeHealth(healthUnit * Amount);
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Inventory>(out var entityInventory)) return;

        if (!entityInventory.CanRecollect) return;

        Recolect(entityInventory);
    }
}
