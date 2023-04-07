using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Recolectable
{
    public int Amount { get; set; }

    public void Recolect(Inventory inventory);

    public void OnTriggerEnter(Collider other);
}
