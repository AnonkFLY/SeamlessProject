using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnonSocket.Data;
using UnityEngine;

public class PlayerList
{
    public static SelfPlayer selfPlayer;
    public static Action<SelfPlayer> onInitSelf;
    private GameObject _playerPrefab;
    private GameObject _selfPrefab;
    private ulong _playerUID;
    private Dictionary<ulong, Player> _players = new Dictionary<ulong, Player>();
    public Action<PlayerData> onJoinPlayer;
    public Action<PlayerData> onQuitPlayer;

    public PlayerList(GameObject playerPrefab, GameObject self)
    {
        _playerPrefab = playerPrefab;
        _selfPrefab = self;
        PacketHandlerRegister.RegisterRoomPlayerChange(ReceivePlayerData);
        PacketHandlerRegister.RegisterPosHandler(AyncPosition);
        PacketHandlerRegister.RegisterAsynInfo(AsyncInfo);
        PacketHandlerRegister.RegisterQuitRoom(QuitRoom);
        PacketHandlerRegister.RegisterPickupItem();
        PacketHandlerRegister.ChangePlayerPos(ChangePosHandler);
        PacketHandlerRegister.ChangeWeapon(ChangeWeapon);
        PacketHandlerRegister.GetGameTime(GetGameTimeHandler);
    }

    private void GetGameTimeHandler(PacketBase packetBase)
    {
        var time = packetBase.ReadInt32();
        GameManager.Instance.OnStartGame(time);
    }

    private void ChangeWeapon(PacketBase packet)
    {
        var uid = packet.ReadULong();
        if (_playerUID == uid)
            return;
        var weapon = packet.ReadByte();
        _players.TryGetValue(uid, out var getPlayer);
        var obj = GameManager.Instance.assetManager.GetWeapon((WeaponType)weapon);
        if (obj != null)
            obj = GameObject.Instantiate(obj);
        getPlayer?.SetWeaponModel(obj);
    }

    private void ChangePosHandler(PacketBase packet)
    {
        var pos = packet.ReadVector3();
        selfPlayer.ChangePos(pos);
    }

    private void AsyncInfo(PacketBase packet)
    {
        var uid = packet.ReadULong();
        var health = packet.ReadFloat();
        var armor = packet.ReadFloat();
        var maxHealth = packet.ReadFloat();
        var maxArmor = packet.ReadFloat();
        _players.TryGetValue(uid, out var getPlayer);
        getPlayer?.UpdatePlayerStateInfo(maxArmor, armor, maxHealth, health);
    }

    private void QuitRoom(PacketBase packet)
    {
        GameManager.Instance.OnQuitRoom();
    }
    private void ReceivePlayerData(PacketBase packet)
    {
        var join = packet.ReadBoolean();
        var json = packet.ReadString();
        var pos = packet.ReadVector3();
        var playerData = JsonUtility.FromJson<PlayerData>(json);
        if (join)
        {
            UnityEngine.Debug.Log($"{playerData.GetName()}进入了房间，初始位置{pos}");
            var player = AddPlayer(playerData);
            player?.UpdateData(playerData);
            player.Transform.position = pos;
            onJoinPlayer?.Invoke(playerData);
        }
        else
        {
            UnityEngine.Debug.Log($"{playerData.GetName()}退出了房间");
            RemovePlayer(playerData.uid);
            onQuitPlayer?.Invoke(playerData);
        }
    }

    public void CloseList()
    {
        var list = _players.ToArray();
        foreach (var item in list)
        {
            RemovePlayer(item.Key);
        }
    }

    public void RemovePlayer(ulong uid)
    {
        if (!_players.TryGetValue(uid, out var getValue))
            return;
        _players.Remove(uid);
        GameObject.Destroy(getValue.gameObject);
    }

    public Player AddPlayer(PlayerData data)
    {
        //        UnityEngine.Debug.Log($"尝试加入玩家[{data.uid}]:{data.playerUserName}===本地玩家{ClientHub.Instance.PlayerData.uid}");
        var uid = data.uid;
        if (_players.TryGetValue(uid, out var getValue))
            return getValue;

        if (ClientHub.Instance.PlayerData.uid == uid)
        {
            _playerUID = uid;
            return AddSelf(uid);
        }
        else
        {
            return AddOtherPlayer(uid);
        }
    }
    private Player AddSelf(ulong uid)
    {
        var player = GameObject.Instantiate(_selfPrefab).GetComponent<SelfPlayer>();
        selfPlayer = player;
        _players.Add(uid, player);
        onInitSelf?.Invoke(selfPlayer);
        GameManager.Instance.selfPlayerObj = selfPlayer;
        return player;
    }
    private Player AddOtherPlayer(ulong uid)
    {
        var player = GameObject.Instantiate(_playerPrefab).GetComponent<Player>();
        _players.Add(uid, player);
        return player;
    }
    private void AyncPosition(PacketBase packet)
    {
        var length = packet.ReadInt32();
        Vector3 pos;
        Vector3 dir;
        byte state;
        ulong uid;
        for (int i = 0; i < length; i++)
        {
            uid = packet.ReadULong();
            pos = packet.ReadVector3();
            dir = packet.ReadVector3();
            state = packet.ReadByte();
            _players.TryGetValue(uid, out var getValue);
            if (uid == _playerUID)
            {
                getValue?.SetState(state);
                continue;
            }
            getValue?.SetPosAndEluer(pos, dir, state);
        }
    }
    public void UpdatePlayerData(PlayerData data)
    {
        _players[data.uid].UpdateData(data);
    }
}
