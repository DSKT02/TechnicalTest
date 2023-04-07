using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolectableKey : MonoBehaviour, I_Recolectable
{
    [field: SerializeField] public int Amount { get; set; }

    public void Recolect(Inventory inventory)
    {
        if (!inventory.HasKey)
        {
            inventory.HasKey = true;
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Inventory>(out var entityInventory)) return;

        if (!entityInventory.CanRecollect) return;

        Recolect(entityInventory);
    }
}
