using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Inventory>(out var entityInventory)) return;

        if (!entityInventory.HasKey) return;

        entityInventory.HasKey = false;

        Destroy(gameObject);
    }
}
