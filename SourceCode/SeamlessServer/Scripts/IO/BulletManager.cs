using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Create/BulletManager")]
public class BulletManager : ScriptableObject
{
    [SerializeField]
    GameObject[] objs;
    Transform root;
    public Dictionary<byte, BulletBase> typeObjs = new Dictionary<byte, BulletBase>();
    private Dictionary<byte, Queue<BulletBase>> _bulletsPoolBuffer = new Dictionary<byte, Queue<BulletBase>>();
    private void OnEnable()
    {
        for (int i = 0; i < objs.Length; i++)
        {
            var bullet = objs[i].GetComponent<BulletBase>();
            if (!typeObjs.ContainsKey(bullet.id))
                typeObjs.Add(bullet.id, bullet);
        }
    }
    public BulletBase GetBulletInstance(byte type)
    {
        //如果不存在列表
        if (!_bulletsPoolBuffer.TryGetValue(type, out var getValue))
        {
            var queue = new Queue<BulletBase>();
            _bulletsPoolBuffer[type] = queue;
        }
        else if (getValue.Count > 0)
        {
            var obj = getValue.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        return CreateBulletInstance(type);
    }
    private BulletBase CreateBulletInstance(byte type)
    {
        if (!typeObjs.TryGetValue(type, out var getValue))
        {
            return null;
        }
        var result = Instantiate(getValue.gameObject).GetComponent<BulletBase>();
        result.onDestory += AddPoolBuffer;
        return result;
    }
    private void AddPoolBuffer(BulletBase bullet)
    {
        if (!root)
            root = new GameObject("BulletRoot").transform;
        bullet.gameObject.SetActive(false);
        bullet.transform.SetParent(root);
        _bulletsPoolBuffer[bullet.id].Enqueue(bullet);
    }
}