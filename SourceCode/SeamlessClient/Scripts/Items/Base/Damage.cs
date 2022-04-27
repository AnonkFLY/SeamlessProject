using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Damage
{
    public float damageValue;
    public float trueDamage;
    public Player damageOrigin;

    public Damage()
    {

    }

    public Damage(float damageValue, float trueDamage, Player damageOrigin)
    {
        this.damageValue = damageValue;
        this.trueDamage = trueDamage;
        this.damageOrigin = damageOrigin;
    }
}
