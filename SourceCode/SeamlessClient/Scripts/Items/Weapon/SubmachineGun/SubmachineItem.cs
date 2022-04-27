using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Create/Weapon/SubmachineGun")]
public class SubmachineItem : FirearmsItem
{
    public override void OnGetFirearms()
    {

    }

    public override void OnQuitSelect()
    {
        if (IsReload())
        {
            ReloadTiming = 0;
        }
    }

    public override void OnSelect()
    {

    }

    public override void Reload()
    {
        ReloadAmmon();
        onAmmoChange?.Invoke(currentBullet, reserveAmmo);
    }


    public override void Shoot()
    {
        if (ReloadUpdate())
            return;
        if (Input.GetMouseButton(0) && canShoot)
        {
            if (currentBullet <= 0)
            {
                if (reloadTiming > 0)
                    return;
                if (currentBullet == maxBullet || reserveAmmo == 0)
                    return;
                ReloadTiming = reloadTimer;
                return;
            }
            ShootPacket();
            ShootABullet();
        }
    }
    public void ShootABullet()
    {
        currentBullet--;
        onAmmoChange?.Invoke(currentBullet, reserveAmmo);
    }
}
