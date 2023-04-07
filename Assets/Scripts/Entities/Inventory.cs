using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private bool canRecollect;

    [field: SerializeField]
    public Transform HandLocationParent { get; set; }

    [SerializeField]
    private List<FireWeapon> weapons = new List<FireWeapon>();

    public bool HasKey { get => hasKey; set { hasKey = value; OnKeyChange?.Invoke(hasKey); } }

    public FireWeapon CurrentWeapon
    {
        get => currentWeapon;
        set
        {
            currentWeapon = value;
            OnWeaponChange?.Invoke(currentWeapon);
            foreach (var item in weapons)
            {
                item.gameObject.SetActive(item == currentWeapon);
            }
        }
    }

    public bool CanRecollect { get => canRecollect; }

    public List<FireWeapon> Weapons { get => weapons; }

    private FireWeapon currentWeapon;

    private bool hasKey;

    public Action<FireWeapon> OnWeaponChange { get; set; }

    public Action<bool> OnKeyChange { get; set; }

    private void Start()
    {
        HasKey = false;
        foreach (var item in weapons)
        {
            item.transform.SetParent(HandLocationParent);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }
        if (weapons.Count != 0)
        {
            CurrentWeapon = weapons[0];
        }
    }
}
