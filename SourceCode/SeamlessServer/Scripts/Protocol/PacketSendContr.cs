using System.Numerics;
using AnonSocket.AnonServer;
using AnonSocket.Data;

public static class PacketSendContr
{
    /// <summary>
    /// 注册结果
    /// </summary>
    /// <param name="client"></param>
    /// <param name="uid"></param>
    public static void SendRegisterHandler(Client client, ulong uid)
    {
        var packet = new PacketBase(PacketSendIDOnServer.UserRegister, 16);
        packet.Write(uid);
        client.SendMessage(packet);
    }
    /// <summary>
    /// 登录结果
    /// </summary>
    /// <param name="client"></param>
    /// <param name="uid"></param>
    public static void SendLoginHandler(Client client, ulong uid)
    {
        var packet = new PacketBase(PacketSendIDOnServer.UserLogin, 16);
        packet.Write(uid);
        client.SendMessage(packet);
    }
    /// <summary>
    /// 发送用户的数据
    /// </summary>
    /// <param name="client"></param>
    /// <param name="data"></param>
    public static void SendPlayerDataHandler(Client client, PlayerData data)
    {
        var packet = new PacketBase(-5);
        packet.Write(data.GetJsonString());
        client.SendMessage(packet);
    }
    /// <summary>
    /// 发送房间的基本数据
    /// </summary>
    /// <param name="client"></param>
    /// <param name="packet"></param>
    public static void SendRoomData(Client client, PacketBase packet)
    {
        client.SendMessage(packet);
    }
    /// <summary>
    /// 发送请求进入房间的结果
    /// </summary>
    /// <param name="client"></param>
    /// <param name="result"></param>
    public static void SendJoinResult(Client client, int result, RoomData data)
    {
        var packet = new PacketBase(-11);
        packet.Write(result);
        packet.Write(data.GetString());
        client.SendMessage(packet);
    }
    public static void SendJoinDone(Client client, int roomID)
    {
        var packet = new PacketBase(-15, 12);
        packet.Write(roomID);
        client.SendMessage(packet);
    }
    /// <summary>
    /// 发送房间内的一个玩家数据
    /// </summary>
    /// <param name="client"></param>
    /// <param name="data"></param>
    public static void SendInRoomPlayerData(Client client, PacketBase packet)
    {
        client.SendMessage(packet);
    }
    public static void AyncPosition(Client client, PacketBase packet)
    {
        client.SendMessage(packet);
    }
    public static void QuitRoomPlayer(Client client)
    {
        var packet = new PacketBase(-16, 8);
        client.SendMessage(packet);
    }
    /// <summary>
    /// valueType
    /// 0-delete-1-kill-2-dead-3-score-4-gold
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="valueType"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static PacketBase GetRoomPlayerDataChange(ulong uid, byte valueType, int value)
    {
        var packet = new PacketBase(-24, 21);
        //valueType
        //0-delete-1-kill-2-dead-3-score-4-gold
        packet.Write(uid);
        packet.Write(valueType);
        packet.Write(value);
        return packet;
    }
    public static void CreateOrDeletetem(Client client, PacketBase packet)
    {
        client.SendMessage(packet);
    }
}