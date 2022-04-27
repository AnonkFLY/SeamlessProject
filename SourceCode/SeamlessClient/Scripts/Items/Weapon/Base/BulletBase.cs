using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour, IGetAssetsType<BulletType>
{
    public BulletType type;
    private Rigidbody _rig;
    private Transform _transform;
    public Action<BulletBase> onDestory;
    private TrailRenderer _trailRenderer;
    private WaitForSeconds _wait = new WaitForSeconds(5);
    [SerializeField] private float addForce = 1;
    private void Awake()
    {
        _rig = GetComponent<Rigidbody>();
        _trailRenderer = GetComponentInChildren<TrailRenderer>();
        _transform = transform;
    }
    public void InitBullet(Vector3 pos, Vector3 addForce)
    {
        _rig.velocity = Vector3.zero;
        _transform.position = pos;
        _trailRenderer.Clear();
        _transform.eulerAngles = addForce.normalized;
        _rig.velocity = addForce;
        StartCoroutine(DelayDestroy());
    }
    private void OnCollisionEnter(Collision other)
    {
        other.transform.GetComponent<IHurtable>()?.BeHurt(this);
        other.transform.GetComponent<Player>()?.OnDeadForce(other.impulse);
        OnDestoryPool();
    }
    private void OnDestoryPool()
    {
        if (!gameObject.activeInHierarchy)
            return;
        StopAllCoroutines();
        //other.gameObject.GetComponent<IHeart>();
        onDestory?.Invoke(this);
    }
    private IEnumerator DelayDestroy()
    {
        yield return _wait;
        OnDestoryPool();
    }

    BulletType IGetAssetsType<BulletType>.GetAssetsType()
    {
        return type;
    }
}
public enum BulletType
{
    PistolBullet,
    RifleBullet,
    ArmorPiercingProjectile,
    SubmachineGunBullet
}