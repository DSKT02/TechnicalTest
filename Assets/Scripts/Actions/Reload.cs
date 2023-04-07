using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reload : EntityAction<bool>
{
    [SerializeField]
    private Inventory inventory;

    public override void SetInput(bool _input)
    {
        base.SetInput(_input);
        Execute();
    }

    protected override void Action()
    {
        inventory.CurrentWeapon.Reload();
    }
}
