using AnonSocket.Data;
using UnityEngine;

public static class PacketSendContr
{
    private static ClientHub _clientHub = ClientHub.Instance;
    public static void SendRegisterData(LoginData data)
    {
        PacketBase packet = new PacketBase(PacketSendIDOnClient.RegisterID);
        packet.Write(data.userName);
        packet.Write(data.passWord.MD5());
        //packet.Write(data.passWord);
        packet.Write(data.passWordAgain);
        _clientHub.SendPacket(packet);
    }
    public static void PickupAsk()
    {
        var packet = new PacketBase(-22, 8);
        _clientHub.SendPacket(packet);
    }
    public static void SendMessageString(string message)
    {
        var packet = new PacketBase(-23);
        packet.Write(message);
        _clientHub.SendPacket(packet);
    }
    public static void SendLoginData(LoginData data)
    {
        PacketBase packet = new PacketBase(PacketSendIDOnClient.LoginID);
        packet.Write(data.userName);
        packet.Write(data.passWord.MD5());
        //packet.Write(data.passWord);
        _clientHub.SendPacket(packet);
    }
    public static void SendCreateRoom(PacketBase packet)
    {
        _clientHub.SendPacket(packet);
    }
    public static void SendJoinRoom(int roomID)
    {
        var playerData = ClientHub.Instance.PlayerData;
        if (playerData == null)
        {
            UnityEngine.Debug.LogError("玩家数据为空，无法登录");
            return;
        }

        PacketBase packet = new PacketBase(PacketSendIDOnClient.JoinRoom, 20);
        packet.Write(roomID);
        packet.Write(playerData.uid);
        _clientHub.SendPacket(packet);
    }
    public static void SendJoinRoom(RoomData room)
    {
        var playerData = ClientHub.Instance.PlayerData;
        if (playerData == null)
        {
            UnityEngine.Debug.LogError("玩家数据为空，无法登录");
            return;
        }
        if (room == null)
        {
            UnityEngine.Debug.LogError("房间数据为空，无法进入");
            return;
        }
        PacketBase packet = new PacketBase(PacketSendIDOnClient.JoinRoom, 20);
        packet.Write(room.roomID);
        packet.Write(playerData.uid);
        _clientHub.SendPacket(packet);
    }
    public static void SendQuitRoom()
    {
        var packet = new PacketBase(-13, 20);
        packet.Write(ClientHub.Instance.PlayerData.uid);
        packet.Write(114514);
        _clientHub.SendPacket(packet);
    }
    /// <summary>
    /// 请求房间列表数据
    /// </summary>
    public static void AskRoomsData()
    {
        var packet = new PacketBase(-6, 8);
        _clientHub.SendPacket(packet);
    }
    public static void SendInput(ulong uid, Vector3 pos, Vector3 dir, byte state)
    {
        var packet = new PacketBase(-10, 45);
        packet.Write(uid);
        packet.Write(pos);
        packet.Write(dir);
        packet.Write(state);
        _clientHub.SendPacket(packet);
    }
    /// <summary>
    /// 进入了房间
    /// </summary>
    public static void SendClientJoinRoom()
    {
        var packet = new PacketBase(-18, 8);
        _clientHub.SendPacket(packet);
    }
    public static void SendGameName(string name)
    {
        var packet = new PacketBase(-19);
        packet.Write(name);
        _clientHub.SendPacket(packet);
    }
    public static void SendChangePassword(string newPassword)
    {
        var packet = new PacketBase(-20);
        packet.Write(newPassword);
        _clientHub.SendPacket(packet);
    }
    public static void ShootBullet(BulletType type, Transform firePos)
    {
        var packet = new PacketBase(-21, 43);
        packet.Write(_clientHub.PlayerData.uid);
        packet.Write((byte)type);
        packet.Write(firePos.position);
        packet.Write(firePos.forward);
        _clientHub.SendPacket(packet);
    }
    public static void SendSignOut()
    {
        var i = _clientHub.PlayerData.uid;
        var packet = new PacketBase(-3, 16);
        _clientHub.SendPacket(packet);
    }
    public static void SendThrowItem(Vector3 pos, byte otherID, int ammon, int current)
    {
        var packet = new PacketBase(-25, 29);
        packet.Write(pos);
        packet.Write(otherID);
        packet.Write(ammon);
        packet.Write(current);
        _clientHub.SendPacket(packet);
    }
    public static void SendWeaponChange(byte weapon)
    {
        var packet = new PacketBase(-26, 9);
        packet.Write(weapon);
        _clientHub.SendPacket(packet);
    }
}