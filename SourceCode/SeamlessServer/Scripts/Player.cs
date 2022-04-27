using System;
using System.Collections;
using System.Collections.Generic;
using AnonSocket.Data;
using UnityEngine;

public class Player : MonoBehaviour, IHurtable
{
    private Transform _transform;
    [SerializeField] private bool isDelay = true;
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private byte _state = 0;
    [SerializeField]
    private byte _weapon = 0;
    [SerializeField]
    private float speed = 3f;
    [SerializeField] private float _hurtDelay = 0.3f;
    [SerializeField] private PlayerInfo _info = new PlayerInfo(80, 10);
    public List<ItemBase> onGetItem = new List<ItemBase>();
    public Transform Transform { get => _transform; set => _transform = value; }
    public PlayerData PlayerData { get => _playerData; set => _playerData = value; }
    public PlayerInfo Info { get => _info; }
    /// <summary>
    /// dead player - orgin player
    /// </summary>
    public Action<Player, Player> onDead;
    public Action<Player> onRenascence;
    protected bool isHurt;

    private void Awake()
    {
        _transform = transform;
        speed = speed * ServerHub.Instance.tick;
        _info.onArmorBreak += () =>
        {
            if (_state == 1)
                return;
            StartCoroutine(RecoverArmor());
        };
        _info.onDead += DeadEffect;
    }

    private void DeadEffect(Player damage)
    {
        if (_state != 0)
            return;
        _state = 1;
        onDead?.Invoke(this, damage);
        StartCoroutine(Renascence());
    }

    private IEnumerator Renascence()
    {
        yield return new WaitForSeconds(5);
        RenascenceEffect();
    }
    private void RenascenceEffect()
    {
        _state = 0;
        onRenascence?.Invoke(this);
    }
    public void AddItem(ItemBase item)
    {
        onGetItem.Add(item);
    }
    public void Remove(ItemBase item)
    {
        onGetItem.Remove(item);
    }

    private IEnumerator RecoverArmor()
    {
        var wait = new WaitForSeconds(3);
        while (true)
        {
            yield return wait;
            _info.AddArmor(2);
            if (_info.IsMaxArmor())
                break;
            if (_state == 1)
                break;
        }
    }
    public void Move(Vector3 pos, Vector3 dir, byte state)
    {
        // if (isHurt)
        //     return;
        //UnityEngine.Debug.Log($"收到{_playerData.playerUserName}的位置包");
        if (state != 0)
            return;
        if (!_transform)
            return;
        _transform.localPosition = pos;
        _transform.eulerAngles = dir;
        if (pos.y <= -10)
            _info.ToDead();
    }
    public void GetPosAndEluer(ref PacketBase packet)
    {
        packet.Write(_transform.localPosition);
        packet.Write(_transform.eulerAngles);
        packet.Write(_state);
    }
    public void AysnWeapon(byte weaponType)
    {
        _weapon = weaponType;
    }
    public void OnGetItem()
    {
        if (onGetItem.Count >= 1)
        {
            onGetItem[0].OnGet(this);
        }
    }

    public void BeHurt(BulletBase bullet)
    {
        // isHurt = true;
        // if (isDelay)
        //     StartCoroutine(DelayHurt());
        // else
        //     isHurt = false;
        if (!bullet.isDamage)
            return;
        _info.Damage(bullet.damage);
    }
    private IEnumerator DelayHurt()
    {
        yield return new WaitForSeconds(_hurtDelay);
        isHurt = false;
    }
}
