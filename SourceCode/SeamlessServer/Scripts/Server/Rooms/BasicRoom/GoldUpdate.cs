using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldUpdate : MonoBehaviour
{
    public Vector3 pos;
    public bool has;
    public byte otherData;
    private void Awake()
    {
        pos = transform.localPosition + Vector3.up * 3.5f;
    }
    public void InGold(ItemBase item)
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
