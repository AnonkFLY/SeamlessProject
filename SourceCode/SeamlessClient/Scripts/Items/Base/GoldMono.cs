using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GoldMono : ItemMonoBase
{
    [SerializeField] private Vector3 speed;
    [SerializeField] private Material[] materials;
    private Transform _goldModel;

    public override void InitOtherData(byte otherData)
    {
        GetComponentInChildren<Renderer>().material = materials[otherData];
    }

    private void Awake()
    {
        _goldModel = transform.GetChild(0);
    }

    void Update()
    {
        _goldModel.Rotate(speed * Time.deltaTime, Space.Self);
    }

}
