using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWeapon : MonoBehaviour
{
    [SerializeField] private Ammunition ammunitionPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private CharacterAffiliation shooter;
    [SerializeField] private float fireRate;
    [SerializeField] private int clipSize;
    [SerializeField] private int ammunitionCapacity;
    [SerializeField] private float reloadTime;
    [SerializeField] private bool infiniteAmmo = false;
    [SerializeField] private bool startWithAmmo = false;

    private int currentBullets = 0;
    private int currentReserve = 0;

    private float reloadProgress;
    private bool isReloading = false;
    private bool isInCoolDown = false;

    public Action OnShooting { get; set; }
    public Action<float> OnRealoading { get; set; }
    public Action<int> OnCurrentBulletsChange { get; set; }
    public Action<int> OnCurrentReserveChange { get; set; }

    public int CurrentBullets { get => currentBullets; set { currentBullets = Mathf.Clamp(value, 0, clipSize); OnCurrentBulletsChange?.Invoke(currentBullets); } }
    public int CurrentReserve { get => currentReserve; set { currentReserve = Mathf.Clamp(value, 0, ammunitionCapacity); OnCurrentReserveChange?.Invoke(currentReserve); } }
    public float ReloadProgress { get => reloadProgress; set { reloadProgress = Mathf.Clamp01(value); OnRealoading?.Invoke(reloadProgress); } }

    public bool CanShoot { get => !isReloading && CurrentBullets > 0; }

    public Ammunition AmmunitionType { get => ammunitionPrefab; }

    public int AmmunitionCapacity { get => ammunitionCapacity; }

    private void Start()
    {
        if (!startWithAmmo) return;
        CurrentReserve = ammunitionCapacity / 3;
        CurrentBullets = clipSize;
    }

    public bool Shoot()
    {
        if (isReloading)
        {
            return false;
        }
        if (CurrentBullets == 0)
        {
            Reload();
            return false;
        }
        if (!isInCoolDown)
        {
            var bullet = PoolController.Get(ammunitionPrefab, shootPoint.position, shooter.transform.rotation);
            bullet.Shoot(shooter, shooter.transform.forward);
            CurrentBullets--;
            OnShooting?.Invoke();
            StartCoroutine(C_BulletCoolingDown());
        }
        return true;
    }

    public void Reload()
    {
        if (isReloading || clipSize == CurrentBullets || currentReserve == 0) return;
        StartCoroutine(C_Realoading());
    }

    private IEnumerator C_Realoading()
    {
        if (isReloading) yield break;
        isReloading = true;
        float elapsedTime = 0;
        while (elapsedTime < reloadTime)
        {
            ReloadProgress = elapsedTime / reloadTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        ReloadProgress = 1;

        if (infiniteAmmo)
        {
            CurrentBullets = clipSize;
            isReloading = false;
            yield break;
        }

        int ammunitionNeeded = clipSize - CurrentBullets;
        if (CurrentReserve > ammunitionNeeded)
        {
            CurrentReserve -= ammunitionNeeded;
            CurrentBullets = clipSize;
        }
        else
        {
            CurrentBullets = CurrentReserve;
            CurrentReserve = 0;
        }
        isReloading = false;
    }

    public void GetAmmo(int amount)
    {
        CurrentReserve += amount;
    }

    private IEnumerator C_BulletCoolingDown()
    {
        if (isInCoolDown) yield break;
        isInCoolDown = true;
        yield return new WaitForSeconds(1f / (fireRate <= 0 ? Mathf.Epsilon : fireRate));
        isInCoolDown = false;
    }

}
