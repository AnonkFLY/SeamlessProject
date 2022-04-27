using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUpdate : MonoBehaviour
{
    [SerializeField] int[] weaponIs = new int[4] { 10, 20, 20, 30 };
    public Vector3 pos;
    public bool has;
    private void Awake()
    {
        pos = transform.localPosition + Vector3.up * 3.5f;
    }
    public byte GetWeaponType()
    {
        int r = Random.Range(0, 100);
        for (byte i = 0; i < weaponIs.Length; i++)
        {
            r -= weaponIs[i];
            if (r < 0)
                return i;
        }
        return 10;
    }
    public void InWeapon(ItemBase item)
    {
        item.onGetEvent += Get;
        has = true;
    }
    private void Get(ItemBase item)
    {
        has = false;
        item.onGetEvent -= Get;
    }
}
