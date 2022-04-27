using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodBottle : ItemBase, IUserable
{
    [SerializeField] private float cooling = 10;
    [SerializeField] private float treatValue = 15;

    public float GetCooling()
    {
        return cooling;
    }
    public void UseItem(Player player)
    {
        //layer.AddHealht(15);
        UnityEngine.Debug.Log("Use bloodbottle");
    }
}
