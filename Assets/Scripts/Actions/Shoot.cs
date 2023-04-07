using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : EntityAction<bool>
{
    [SerializeField]
    private EntityAction<Vector2> aimAction;

    [SerializeField]
    private Inventory inventory;

    public override void SetInput(bool _input)
    {
        base.SetInput(_input);
        Execute();
    }

    protected override void Action()
    {
        AllowActionEvent = true;

        if (inventory.CurrentWeapon == null)
        {
            AllowActionEvent = false;
            aimAction.AllowAction = false;
            return;
        }

        aimAction.AllowAction = inventory.CurrentWeapon.CanShoot;
        aimAction.Execute();

        if (inventory.CurrentWeapon.Shoot())
        {
            aimAction.AllowAction = CurrentInput;
        }
        else
        {
            ActionEvent?.Invoke(false);
            AllowActionEvent = false;
        }
    }
}
