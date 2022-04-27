using System;
using System.Collections;
using System.Collections.Generic;
using AnonSocket.Data;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class BulletBase : MonoBehaviour
{
    public float addForce;
    public byte id;
    public Action<BulletBase> onDestory;
    public Damage damage;
    private Transform _trans;
    private Rigidbody _rig;
    private WaitForSeconds wait = new WaitForSeconds(5);
    private bool isEnable = true;
    public bool isDamage;

    private void Awake()
    {
        _trans = transform;
        _rig = GetComponent<Rigidbody>();
    }

    public void Init(RoomBase room, Vector3 pos, Vector3 dir, Player damageOrigin)
    {
        isEnable = true;
        isDamage = room._isOpenDamage;
        damage.damageOrigin = damageOrigin;
        _trans.SetParent(room.Scenes.transform);
        _trans.localPosition = pos;
        _trans.eulerAngles = dir;
        var overAddForce = dir * addForce;
        _rig.velocity = overAddForce;
        room.CreateBullet(id, pos, overAddForce);
        StartCoroutine(DelayDestroy());
    }
    private void OnCollisionEnter(Collision other)
    {
        if (!isEnable)
            return;
        other.transform.GetComponent<IHurtable>()?.BeHurt(this);
        isEnable = false;
        DestroyThis();
    }
    private IEnumerator DelayDestroy()
    {
        yield return wait;
        DestroyThis();
    }
    private void DestroyThis()
    {
        if (!gameObject.activeInHierarchy)
            return;
        StopAllCoroutines();
        onDestory?.Invoke(this);
    }
}
