using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : ItemMonoBase
{
    private PicoIcon icon;
    private Transform itemModel;
    private Transform lockTrans;
    [SerializeField] Vector3 rotation;
    private void Awake()
    {
        icon = FindObjectOfType<PicoIcon>();
        lockTrans = transform.GetChild(0);
    }

    public override void InitOtherData(byte otherData)
    {
        var obj = GameManager.Instance.assetManager.GetWeapon((WeaponType)otherData);
        if (obj == null)
        {
            Destroy(gameObject);
            return;
        }
        Instantiate(obj, lockTrans);
    }

    void Update()
    {
        lockTrans.Rotate(rotation * Time.deltaTime, Space.World);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<SelfPlayer>())
            return;
        icon?.Open();
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<SelfPlayer>())
            return;
        icon?.Close();
    }
}
