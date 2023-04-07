using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponsUI : MonoBehaviour
{
    [SerializeField]
    private Inventory inventory;

    [SerializeField]
    private FireWeapon weapon1, weapon2;

    [SerializeField]
    private CanvasGroup[] weapon1Buttons, weapon2Buttons;

    [SerializeField]
    private TextMeshProUGUI[] bulletsWeapon1, bulletsWeapon2;

    [SerializeField]
    private Image[] reloadIndicators;

    private float realoadPercentageWeapon;

    private string currentBulletsWeapon1, currentReserveWeapon1;
    private string currentBulletsWeapon2, currentReserveWeapon2;

    private void Start()
    {
        inventory.OnWeaponChange += SwitchWeapons;
        inventory.Weapons[0].OnCurrentBulletsChange += (_) => { currentBulletsWeapon1 = _.ToString(); UpdateBullets(); };
        inventory.Weapons[0].OnCurrentReserveChange += (_) => { currentReserveWeapon1 = _.ToString(); UpdateBullets(); };
        inventory.Weapons[1].OnCurrentBulletsChange += (_) => { currentBulletsWeapon2 = _.ToString(); UpdateBullets(); };
        inventory.Weapons[1].OnCurrentReserveChange += (_) => { currentReserveWeapon2 = _.ToString(); UpdateBullets(); };

        inventory.Weapons[0].OnRealoading += (_) => { realoadPercentageWeapon = _; UpdateRechargeIndicator(); };
        inventory.Weapons[1].OnRealoading += (_) => { realoadPercentageWeapon = _; UpdateRechargeIndicator(); };

        realoadPercentageWeapon = 1;
        UpdateRechargeIndicator();
    }

    public void SwitchWeapons(FireWeapon weapon)
    {
        foreach (var item in weapon1Buttons)
        {
            item.alpha = weapon == weapon1 ? 1 : 0.4f;
            item.interactable = weapon == weapon1 ? false : true;
        }

        foreach (var item in weapon2Buttons)
        {
            item.alpha = weapon == weapon2 ? 1 : 0.4f;
            item.interactable = weapon == weapon2 ? false : true;
        }

        foreach (var item in bulletsWeapon1)
        {
            item.gameObject.SetActive(weapon == weapon1);
        }

        foreach (var item in bulletsWeapon2)
        {
            item.gameObject.SetActive(weapon == weapon2);
        }

        realoadPercentageWeapon = 1;
    }

    public void UpdateBullets()
    {
        foreach (var item in bulletsWeapon1)
        {
            item.text = $"{currentBulletsWeapon1}/{currentReserveWeapon1}";
        }
        foreach (var item in bulletsWeapon2)
        {
            item.text = $"{currentBulletsWeapon2}/{currentReserveWeapon2}";
        }
    }

    public void UpdateRechargeIndicator()
    {
        foreach (var item in reloadIndicators)
        {
            item.fillAmount = realoadPercentageWeapon;
            if (realoadPercentageWeapon == 1)
            {
                item.color = new Color(item.color.r, item.color.g, item.color.b, 0);
            }
            else
            {
                item.color = new Color(item.color.r, item.color.g, item.color.b, 1);
            }
        }
    }
}
