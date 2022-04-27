using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmachineMono : WeaponBase
{

    public override void FireEffect()
    {
        //print("开火动画");
    }

    public override void Init()
    {
        InitItem<SubmachineItem>();
    }

    public override void QuitEvent()
    {
        //print("退出");
    }

    public override void ReloadEffect()
    {
        //print("装弹");
    }
}
