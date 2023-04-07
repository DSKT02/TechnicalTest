using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapWeapon : EntityAction<int>
{
    [SerializeField]
    private Inventory inventory;

    public override void SetInput(int _input)
    {
        base.SetInput(_input);
        Execute();
    }

    protected override void Action()
    {
        if (inventory.Weapons.Count > 0 && CurrentInput < inventory.Weapons.Count)
        {
            inventory.CurrentWeapon = inventory.Weapons[CurrentInput];
        }
    }
}
