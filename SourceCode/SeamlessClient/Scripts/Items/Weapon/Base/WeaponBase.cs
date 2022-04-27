using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, IGetAssetsType<WeaponType>
{
    [SerializeField] protected FirearmsItem itemData;

    public FirearmsItem ItemData { get => itemData; }
    public abstract void Init();
    private void Awake()
    {
        Init();

    }
    public abstract void FireEffect();
    public abstract void ReloadEffect();
    public abstract void QuitEvent();

    WeaponType IGetAssetsType<WeaponType>.GetAssetsType()
    {
        var type = (IGetAssetsType<WeaponType>)itemData;
        return type.GetAssetsType();
    }
    protected void InitItem<T>() where T : FirearmsItem
    {
        var itemData = (T)ScriptableObject.CreateInstance(typeof(T));
        this.itemData = this.itemData.Clone<T>(itemData);
        this.itemData.gameObject = this.gameObject;
        this.itemData.firePos = transform.Find("FirePos");
    }
}
