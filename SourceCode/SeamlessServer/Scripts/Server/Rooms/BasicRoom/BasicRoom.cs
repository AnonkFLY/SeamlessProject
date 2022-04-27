using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnonSocket.AnonServer;
using AnonSocket.Data;
using UnityEngine;

public class BasicRoom : RoomBase
{
    private int _itemID = 1;
    private Dictionary<int, ItemBase> objs = new Dictionary<int, ItemBase>();
    public Scoreboard scoreboard;

    public BasicRoom(string homeowener, string roomName, int playerCount, string describ, RoomType type = RoomType.Game1)
    {
        roomData = new RoomData(homeowener, roomName, playerCount, RoomType.Game1, describ);
        roomData.type = type;
        scoreboard = new Scoreboard(this);
        scoreboard.onScoreboardChange += OnScoreChange;
        onPlayerJoinDone += OnJoinPlayerSendAllItem;
        onPlayerDead += OnPlayerDeadHandler;
        onGaming += GamingHandler;
        onStartGame += StartGame;
    }
    private void StartGame()
    {
        //ServerHub.Instance.StartCoroutine(DestroyAll());
        var list = objs.ToArray();
        foreach (var item in list)
        {
            DestroyItem(item.Value);
        }
        AllPlayerBirth();
    }
    private void AllPlayerBirth()
    {
        foreach (var item in playerList)
        {
            var pos = birthPoint[UnityEngine.Random.Range(1, birthPoint.Length - 1)].localPosition + Vector3.up * 4.5f;
            SetPlayerPos(item.Key, pos);
        }
    }
    // IEnumerator DestroyAll()
    // {
    //     var wait = new WaitForEndOfFrame();
    //     foreach (var item in objs)
    //     {
    //         DestoryItem(item.Value);
    //         yield return wait;
    //     }
    // }

    private void OnPlayerDeadHandler(Player player, Player playerOrigin)
    {
        if (playerOrigin == null)
        {
            scoreboard.AddDeadCount(player.PlayerData.uid, 1);
            return;
        }
        var uid = playerOrigin.PlayerData.uid;
        scoreboard.AddDeadCount(player.PlayerData.uid, 1);
        scoreboard.AddKillCount(uid, 1);
        playerOrigin.Info.AddHealth(20);
        playerOrigin.Info.AddMaxArmor();
        scoreboard.AddPlayerScore(uid, 1);
    }

    private void OnScoreChange(ulong uid, byte valueType, int value)
    {
        var packet = PacketSendContr.GetRoomPlayerDataChange(uid, valueType, value);
        if (valueType == 4)
        {
            if (playerList.TryGetValue(uid, out var getValue))
                getValue.Client.SendMessage(packet);
            return;
        }
        BroadCast(packet);
    }

    internal void DestroyItem(ItemBase itemBase)
    {
        //        UnityEngine.Debug.Log("摧毁" + itemBase.name);
        objs.Remove(itemBase.itemID);
        BroadCast(itemBase.GetDestoryPacket());
        if (itemBase.gameObject != null)
            GameObject.Destroy(itemBase.gameObject);
    }
    private int i = 3;
    public void Test()
    {
        // CreateItem(1, Vector3.forward * i + Vector3.up, 1);
        // i++;
        // CreateItem(1, Vector3.forward * i + Vector3.up, 2);
        // i++;
        // CreateItem(1, Vector3.forward * i + Vector3.up, 3);
        // i++;
        // CreateItem(1, Vector3.forward * i + Vector3.up, 4);

    }
    public ItemBase CreateItem(int id, Vector3 pos, byte otherData = 0, int ammon = -1, int current = -1)
    {
        var obj = ServerHub.Instance.GetItems(id);
        var instanceObj = GameObject.Instantiate(obj, Scenes.transform);
        instanceObj.transform.localPosition = pos;
        var item = instanceObj.GetComponent<ItemBase>();
        item.otherData = otherData;
        item.SetData(ammon, current);
        item.itemID = _itemID++;
        objs.Add(item.itemID, item);
        item.onRoom = this;

        SendAllClientCreateItem(item);
        return item;
    }
    private void OnJoinPlayerSendAllItem(Client player)
    {
        Test();
        //        UnityEngine.Debug.Log($"RoomName{roomData.roomName},item count {objs.Count}");
        foreach (var item in this.objs)
        {
            var packet = item.Value.GetPacket();
            player.SendMessage(packet);
        }
    }
    private void SendAllClientCreateItem(ItemBase item)
    {
        var packet = item.GetPacket();
        BroadCast(packet);
    }
    int weaponTime;
    int goldTime;
    private void GamingHandler(int time)
    {
        weaponTime--;
        if (weaponTime <= 0)
        {
            var count = Mathf.Clamp(maxWeaponCount / (weapnCount + 1), 1, 2);
            CreateWeapon(count);
            weaponTime = 10;
        }
        goldTime--;
        if (goldTime <= 0)
        {
            var count2 = Mathf.Clamp(maxGoldCount / (goldCount + 1), 1, 3);
            CreateGold(count2);
            goldTime = 2;
        }
    }
    public int goldCount = 0;
    int maxGoldCount = 20;
    public int weapnCount = 0;
    int maxWeaponCount = 5;
    private void CreateGold(int count)
    {
        for (int i = 0; i < count;)
        {
            if (goldCount >= maxGoldCount)
                return;
            if (CreateGold())
                i++;
        }
    }
    private void CreateWeapon(int count)
    {
        for (int i = 0; i < count;)
        {
            if (weapnCount >= maxWeaponCount)
                return;
            if (CreateWeapon())
                i++;
        }
    }
    private bool CreateWeapon()
    {
        var r = UnityEngine.Random.Range(1, weaponPoint.Length - 1);
        var weapon = weaponPoint[r];
        if (weapon.has)
            return false;
        var pos = weapon.pos;
        var type = weapon.GetWeaponType();
        type++;
        if (type == 10)
            return true;
        var item = CreateItem(1, pos, type);
        weapnCount++;
        item.onGetEvent += RemoveWeaponCount;
        weapon.InWeapon(item);
        return true;
    }
    private void RemoveWeaponCount(ItemBase item)
    {
        item.onGetEvent -= RemoveWeaponCount;
        weapnCount--;
    }
    private void RemoveGoldCount(ItemBase item)
    {
        item.onGetEvent -= RemoveGoldCount;
        goldCount--;
    }
    private bool CreateGold()
    {
        var r = UnityEngine.Random.Range(1, goldPoint.Length - 1);
        var gold = goldPoint[r];
        if (gold.has)
            return false;
        var pos = goldPoint[r].pos;
        goldCount++;
        var item = CreateItem(0, pos, gold.otherData);
        item.onGetEvent += RemoveGoldCount;
        gold.InGold(item);

        return true;
    }
}
