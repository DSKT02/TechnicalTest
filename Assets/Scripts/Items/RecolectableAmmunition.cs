using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolectableAmmunition : MonoBehaviour, I_Recolectable
{
    [SerializeField]
    private Ammunition ammunitionType;

    [field: SerializeField] public int Amount { get; set; }

    public void Recolect(Inventory inventory)
    {
        foreach (var item in inventory.Weapons)
        {
            if (item.AmmunitionType.GetType() == ammunitionType.GetType())
            {
                if (item.CurrentReserve < item.AmmunitionCapacity)
                {
                    item.GetAmmo(Amount);
                    Destroy(gameObject);
                }
                break;
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Inventory>(out var entityInventory)) return;

        if (!entityInventory.CanRecollect) return;

        Recolect(entityInventory);
    }
}
