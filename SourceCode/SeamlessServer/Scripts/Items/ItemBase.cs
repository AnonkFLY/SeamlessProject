using System;
using System.Collections;
using System.Collections.Generic;
using AnonSocket.Data;
using UnityEngine;
using UnityEngine.Events;

public abstract class ItemBase : MonoBehaviour
{
    public int itemID;
    public int id;
    public byte otherData;
    public int ammon;
    public int current;
    public BasicRoom onRoom;
    public Action<ItemBase> onGetEvent;
    /// <summary>
    /// 是碰到就拿
    /// </summary>
    [SerializeField] protected bool triggerGet;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        var player = other.GetComponent<Player>();
        if (!triggerGet)
        {
            OnEnter(player);
            player.AddItem(this);
            return;
        }
        OnGet(player);
    }
    private void OnDestoryThis(Player player)
    {
        player?.Remove(this);
        onRoom?.DestroyItem(this);
        Destroy(gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        var player = other.GetComponent<Player>();
        if (!triggerGet)
        {
            OnExit(player);
            player.Remove(this);
        }
    }
    public virtual PacketBase GetPacket()
    {
        var packet = new PacketBase(-23, 29);
        packet.Write(itemID);
        packet.Write(id);
        packet.Write(transform.localPosition);
        packet.Write(otherData);
        return packet;
    }
    public virtual PacketBase GetDestoryPacket()
    {
        var packet = new PacketBase(-23, 12);
        packet.Write(-itemID);
        return packet;
    }
    public virtual void OnGet(Player player)
    {
        if (this == null)
            return;
        OnGetEvent(player);
        OnDestoryThis(player);
    }
    public abstract void OnGetEvent(Player player);
    public virtual void OnEnter(Player player)
    {

    }
    public virtual void OnExit(Player player)
    {

    }

    internal void SetData(int ammon, int current)
    {
        this.ammon = ammon;
        this.current = current;
        if (ammon == 0 && current == 0)
            StartCoroutine(DelayDestory());
    }
    private IEnumerator DelayDestory()
    {
        yield return new WaitForSeconds(10);
        OnDestoryThis(null);
    }
}
