using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class WeaponManager
{
    private SelfPlayer _player;
    private WeaponSoltView[] _weaponSoltViews;
    //private Action<WeaponSoltView> onSelectWeapon;
    private int _currentIndex;
    private float timer = 0.5f;
    private float timing = 0;
    public bool IsMaxWeapon()
    {
        return GetNullSolt() == -1;
    }
    public WeaponManager(Transform parent)
    {
        GameManager.Instance.onQuitRoom += () =>
        {
            CloseAll();
        };
        _weaponSoltViews = parent.GetComponentsInChildren<WeaponSoltView>();
        _currentIndex = _weaponSoltViews.Length - 1;
        for (int i = _weaponSoltViews.Length - 1; i >= 0; i--)
        {
            _weaponSoltViews[i].InitSolt(this, i);
        }
        UpdateViews(false);
    }
    public void UpdateViews(bool isTrue)
    {
        if (isTrue)
            _weaponSoltViews[_currentIndex].ReSelect();
        else
            _weaponSoltViews[_currentIndex].OnSelect();
    }
    public bool GetWeapon(GameObject item, int ammon, int current)
    {
        var index = GetNullSolt();
        if (index == -1)
            return false;
        var trans = GameManager.Instance.selfPlayerObj.transform.Find("Palm");
        var itemData = GameObject.Instantiate(item, trans).GetComponent<WeaponBase>().ItemData;
        itemData.SetAmmon(ammon, current);
        _weaponSoltViews[index].SetWeaponView(itemData);
        if (index == _currentIndex)
            UpdateViews(true);
        return true;
    }
    public int GetNullSolt()
    {
        for (int i = _weaponSoltViews.Length - 1; i >= 0; i--)
        {
            if (_weaponSoltViews[i].Firearm == null)
                return i;
        }
        return -1;
    }
    public void CloseAll()
    {
        foreach (var item in _weaponSoltViews)
        {
            item.ClearView();
        }
    }
    public void ThrowWeapon(int index)
    {
        var firearmItem = _weaponSoltViews[index].ClearView();
        if (firearmItem == null)
            return;
        PacketSendContr.SendThrowItem(GameManager.Instance.selfPlayerObj.transform.position, (byte)firearmItem.WeaponType, firearmItem.ReserveAmmo, firearmItem.CurrentBullet);
        GameObject.Destroy(firearmItem.gameObject);

        // for (int i = index + 1; i < _weaponSoltViews.Length; i++)
        // {
        //     if (i >= _weaponSoltViews.Length)
        //         break;
        //     if (_weaponSoltViews[i].Firearm != null)
        //     {
        //         _weaponSoltViews[i - 1].SetWeaponView(_weaponSoltViews[i].ClearView());
        //     }
        // }
    }
    public void ThrowWeapon()
    {
        ThrowWeapon(_currentIndex);
    }
    public void OnSelectIndex(int index)
    {
        if (timing > 0)
            return;
        index--;
        if (index == _currentIndex)
            return;
        timing = timer;
        GameManager.Instance.StartCoroutine(ChangeCD());

        _weaponSoltViews[_currentIndex]?.OnQuitSelect();
        //UnityEngine.Debug.Log(_currentIndex);
        _currentIndex = index;
        UpdateViews(false);
    }
    private IEnumerator ChangeCD()
    {
        var wait = new WaitForEndOfFrame();
        while (timing > 0)
        {
            yield return wait;
            timing -= Time.deltaTime;
            foreach (var item in _weaponSoltViews)
            {
                item.ReloadEffect(timing, timer);
            }
        }
    }
}
