using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoData
{
    public string username;
    public int goldCount;
    public LimitValue<float> health;
    public LimitValue<float> armor;
    public LimitValue<float> vigour;
    public float timer = 0;
    public bool RemoveVigour(float value)
    {
        vigour.SetValue(vigour.Value - value);
        timer = 2;
        return vigour.Value > 0;
    }
    public void AddVigour(float value)
    {
        if (timer > 0)
        {
            timer -= value;
            return;
        }
        if (vigour.Value >= vigour.MaxValue)
        {
            vigour.ResetValue();
            return;
        }
        vigour.SetValue(vigour.Value + value);
    }

    public PlayerInfoData(float maxHealth, float maxArmor, float maxVigour)
    {
        health = new LimitValue<float>(maxHealth);
        armor = new LimitValue<float>(maxArmor);
        vigour = new LimitValue<float>(maxVigour);
    }

    public void UpdateInfo(float maxArmor, float armor, float maxHealth, float health)
    {
        this.health.SetMaxValue(maxHealth);
        this.health.SetValue(health);
        this.armor.SetMaxValue(maxArmor);
        this.armor.SetValue(armor);
    }

}

public class LimitValue<T>
{
    private T _value;
    private T _maxValue;
    public event Action<T, T> onValueChange;
    public LimitValue(T maxValue)
    {
        _maxValue = maxValue;
        ResetValue();
    }
    public void ResetValue()
    {
        _value = _maxValue;
        OnValueChangeEvent();
    }
    private void OnValueChangeEvent()
    {
        onValueChange?.Invoke(_value, MaxValue);
    }
    public void SetValue(T value)
    {
        _value = value;
        OnValueChangeEvent();
    }
    public void SetMaxValue(T maxValue)
    {
        _maxValue = maxValue;
        OnValueChangeEvent();
    }
    public T MaxValue { get => _maxValue; }
    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            OnValueChangeEvent();
        }
    }
}