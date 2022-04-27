using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[CreateAssetMenu(menuName = "Create/Weapon")]
public abstract class FirearmsItem : ItemBase, IGetAssetsType<WeaponType>
{
    private void OnEnable()
    {
        InitItem();
    }

    [SerializeField] protected WeaponType weaponType;//
    [SerializeField] protected int maxBullet;//
    [SerializeField] protected int currentBullet;//
    [SerializeField] protected int reserveAmmo;//
    [SerializeField] protected BulletType bullet;//
    [SerializeField] protected float shootTimer;
    [SerializeField] protected float reloadTimer;
    protected float reloadTiming;
    public GameObject gameObject;
    public Action<int, int> onAmmoChange;//current/resrve
    public int MaxBullet { get => maxBullet; }
    public int CurrentBullet { get => currentBullet; }
    public int ReserveAmmo { get => reserveAmmo; }
    public WeaponType WeaponType { get => weaponType; }
    public float ShootTimer { get => shootTimer; }
    public float ReloadTimer { get => reloadTimer; }
    public Action<float, float> onReloadTimerListener;
    public float ReloadTiming { get => reloadTiming; set { reloadTiming = value; onReloadTimerListener?.Invoke(value, reloadTimer); } }

    [HideInInspector] public bool canShoot = true;

    public Transform firePos;
    public abstract void Shoot();
    public abstract void Reload();
    public abstract void OnSelect();
    public abstract void OnQuitSelect();
    public abstract void OnGetFirearms();
    public virtual void InitItem()
    {
        type = ItemType.Weapon;
        currentBullet = maxBullet;
    }
    public virtual T Clone<T>(FirearmsItem weapon) where T : FirearmsItem
    {
        weapon.bullet = this.bullet;
        weapon.maxBullet = this.maxBullet;
        weapon.currentBullet = this.currentBullet;
        weapon.reserveAmmo = this.reserveAmmo;
        weapon.shootTimer = this.shootTimer;
        weapon.reloadTimer = this.reloadTimer;
        weapon.canShoot = this.canShoot;
        weapon.weaponType = this.weaponType;
        weapon.sprite = this.sprite;
        weapon.itemName = this.itemName;
        weapon.type = this.type;
        weapon.describe = this.describe;
        //weapon.firePos = this.firePos;
        return (T)weapon;
    }
    public void SetAmmon(int ammon, int current)
    {
        if (ammon != -1)
            reserveAmmo = ammon;
        if (current != -1)
            currentBullet = current;
    }
    protected void ShootPacket()
    {
        PacketSendContr.ShootBullet(bullet, firePos);
        canShoot = false;
        GameManager.Instance.DelayExecute(shootTimer, () => { canShoot = true; });
    }
    protected void ReloadAmmon()
    {
        var need = maxBullet - currentBullet;
        if (need == 0)
            return;
        if (reserveAmmo == -1)
        {
            currentBullet = maxBullet;
            return;
        }
        if (reserveAmmo >= need)
        {
            reserveAmmo -= need;
            currentBullet += need;
        }
        else
        {
            currentBullet += reserveAmmo;
            reserveAmmo = 0;
        }
    }
    protected bool IsReload()
    {
        if (ReloadTiming <= 0)
            return false;
        ReloadTiming -= Time.deltaTime;
        if (ReloadTiming <= 0)
        {
            Reload();
        }
        return true;
    }
    protected bool ReloadUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (reloadTiming > 0)
                return false;
            if (currentBullet == maxBullet || reserveAmmo == 0)
                return false;
            ReloadTiming = reloadTimer;
        }
        return IsReload();
    }

    WeaponType IGetAssetsType<WeaponType>.GetAssetsType()
    {
        return WeaponType;
    }
}
public enum WeaponType
{
    Null,
    Pistol,
    Rifle,
    ArmorPiercingGun,
    SubmachineGun
}
