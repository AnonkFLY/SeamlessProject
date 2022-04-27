using System;
using System.Collections;
using System.Collections.Generic;
using AnonSocket.Data;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Test : MonoBehaviour
{
    [SerializeField] GameObject test;
    private void Awake()
    {
        Destroy(gameObject, 4);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Destroy(gameObject);
        }
        // if (Input.GetKeyDown(KeyCode.G))
        //     test = GameManager.Instance.assetManager.GetWeapon(WeaponType.Pistol);
        // if (Input.GetKeyDown(KeyCode.T))
        //     GameManager.Instance.weaponManager.GetWeapon(test);
    }
}
