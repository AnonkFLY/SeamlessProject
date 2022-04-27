using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Create/ItemAssets")]
public class ItemManager : AssetsBase<int>
{
    private Dictionary<int, GameObject> _items = new Dictionary<int, GameObject>();
    public void DestroyAllItem()
    {
        var items = _items.ToArray();
        foreach (var item in items)
        {
            DestroyItem(item.Key);
        }
    }
    public void CreateOrDestoryItem(int itemID, int id, Vector3 pos, byte otherData = 0)
    {
        if (itemID > 0)
        {
            CreateItem(itemID, id, pos, otherData);
        }
        else
        {
            DestroyItem(-itemID);
        }
    }
    public void CreateItem(int itemID, int id, Vector3 pos, byte otherData)
    {
        var getValue = GetItemObj(id);
        var obj = GameObject.Instantiate(getValue);
        obj.GetComponent<ItemMonoBase>().InitOtherData(otherData);
        obj.transform.position = pos;
        if (_items.ContainsKey(itemID))
        {
            DestroyItem(itemID);
        }
        _items.Add(itemID, obj);
    }
    public void DestroyItem(int itemID)
    {
        if (!_items.TryGetValue(itemID, out var getValue))
        {
            UnityEngine.Debug.LogError($"错误没有对应{itemID}的物品");
            _items.Remove(itemID);
            return;
        }
        if (getValue != null)
        {
            //            UnityEngine.Debug.Log("Destroy");
            getValue.transform.position = Vector3.up * 10000;
            GameObject.Destroy(getValue);
        }
        _items.Remove(itemID);
    }
    private GameObject GetItemObj(int key)
    {
        if (!typeObjs.TryGetValue(key, out var getValue))
        {
            UnityEngine.Debug.LogError($"错误,没有找到对应{key}的物品");
            return null;
        }
        return getValue;
    }
}
