using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemMonoBase : MonoBehaviour, IGetAssetsType<int>
{
    [SerializeField] protected int id;
    [SerializeField] protected byte otherData;
    public abstract void InitOtherData(byte otherData);
    public int GetAssetsType()
    {
        return id;
    }
}
