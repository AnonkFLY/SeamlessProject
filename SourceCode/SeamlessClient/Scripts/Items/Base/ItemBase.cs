using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ItemBase : ScriptableObject
{
    public string itemName;
    public int id;
    public ItemType type;
    public Sprite sprite;
    public string describe;
}
public enum ItemType
{
    Weapon,
    Prop,
    Skill
}
