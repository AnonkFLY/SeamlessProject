using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using IO;
using UnityEngine;

public class BulletManager
{
    private Transform _bulletRoot;
    private AssetManager _assetManager;
    private Dictionary<BulletType, Queue<BulletBase>> _bulletsPoolBuffer = new Dictionary<BulletType, Queue<BulletBase>>();
    public BulletManager()
    {
        _bulletRoot = new GameObject("BulletRoot").transform;
        _assetManager = GameManager.Instance.assetManager;
    }
    public void CreateBullet(BulletType type, Vector3 pos, Vector3 addForce)
    {
        var bullet = GetBullet(type);
        bullet.InitBullet(pos, addForce);
    }
    private BulletBase GetBullet(BulletType type)
    {
        //如果不存在列表
        if (!_bulletsPoolBuffer.TryGetValue(type, out var getValue))
        {
            var queue = new Queue<BulletBase>();
            _bulletsPoolBuffer[type] = queue;
        }
        else if (getValue.Count > 0)
        {
            var bullet = getValue.Dequeue();
            bullet.gameObject.SetActive(true);
            return bullet;
        }
        return CreateBulletInstance(type);
    }
    private BulletBase CreateBulletInstance(BulletType type)
    {
        var bullet = _assetManager.CreateBullet(type, _bulletRoot);
        //        UnityEngine.Debug.Log("Create Bullet");
        bullet.onDestory += AddPoolBuffer;
        return bullet;
    }
    private void AddPoolBuffer(BulletBase bullet)
    {
        bullet.gameObject.SetActive(false);
        _bulletsPoolBuffer[bullet.type].Enqueue(bullet);
    }
}
