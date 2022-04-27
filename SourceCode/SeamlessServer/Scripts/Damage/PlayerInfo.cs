using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
[System.Serializable]
public class PlayerInfo
{
    public float maxHealth;
    public float health;
    public float maxArmor;
    public float armor;
    /// <summary>
    /// health armor maxHealth maxArmor
    /// </summary>
    public Action<float, float, float, float> onInfoChange;
    public Action<Player> onDead;
    public Action onArmorMax;
    public Action onArmorBreak;
    public void AddHealth(float value)
    {
        //UnityEngine.Debug.Log($"加血:{value}");
        health = Mathf.Clamp(health + value, 0, maxHealth);
        UpdateInfo();
    }
    public void Damage(Damage damage)
    {
        var damageValue = damage.ordinaryDamage - ((armor - damage.trueDamage) / damage.ordinaryDamage);
        var max = IsMaxArmor();
        armor -= damageValue;
        if (armor < 0)
        {
            health += armor;
            armor = 0;
        }
        health -= damage.trueDamage;
        health = Mathf.Clamp(health, 0, maxHealth);
        if (max && !IsMaxArmor())
            onArmorBreak?.Invoke();
        if (health <= 0)
            onDead?.Invoke(damage.damageOrigin);
        UpdateInfo();
    }
    public void ToDead()
    {
        health = 0;
        onDead?.Invoke(null);
        UpdateInfo();
    }
    public bool IsMaxArmor()
    {
        return armor >= maxArmor;
    }
    public void AddMaxArmor()
    {
        maxArmor++;
        AddArmor(1);
        UpdateInfo();
    }
    public void AddArmor(float value)
    {
        armor = Mathf.Clamp(armor + value, 0, maxArmor);
        UpdateInfo();
        if (IsMaxArmor())
            onArmorMax?.Invoke();
    }
    public void Reset()
    {
        health = maxHealth;
        armor = maxArmor;
        UpdateInfo();
    }

    public PlayerInfo(float maxHealth, float maxArmor)
    {
        this.maxHealth = maxHealth;
        this.maxArmor = maxArmor;
        Reset();
    }
    private void UpdateInfo()
    {
        onInfoChange?.Invoke(health, armor, maxHealth, maxArmor);
    }
}
