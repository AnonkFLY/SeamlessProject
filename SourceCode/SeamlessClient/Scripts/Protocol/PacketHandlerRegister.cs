using System;
using UnityEngine;
using AnonSocket;
using AnonSocket.Data;
using System.Runtime.InteropServices;
using System.Text;

public static class PacketHandlerRegister
{
    public static PacketHandler<PacketBase> handler;
    private static ItemManager _itemManager;
    public delegate void PacketHandlerAction(PacketBase action);
    public static void RegisterUserHandler(PacketHandlerAction action)
    {
        handler.RegisterHandler(-3, new PacketHandler<PacketBase>.PacketProcessingEvents(action));
        // _handler.RegisterHandler(-20, new PacketHandler<PacketBase>.PacketProcessingEvents(packet =>
        // {
        //     GameManager.Instance.Message(packet.ReadString());
        // }));
    }
    public static void RegisterLoginHandler(PacketHandlerAction action)
    {
        handler.RegisterHandler(-4, new PacketHandler<PacketBase>.PacketProcessingEvents(action));
    }
    public static void RecivePlayerDataHandler(PacketHandlerAction action)
    {
        handler.RegisterHandler(-5, new PacketHandler<PacketBase>.PacketProcessingEvents(action));
    }

    internal static void ChangeWeapon(object changeWeapon)
    {
        throw new NotImplementedException();
    }

    public static void RegisterRoomDataHandler(PacketHandlerAction action)
    {
        handler.RegisterHandler(-7, new PacketHandler<PacketBase>.PacketProcessingEvents(action));
    }

    public static void RegisterJoinRoomResult(PacketHandlerAction action)
    {
        handler.RegisterHandler(-11, new PacketHandler<PacketBase>.PacketProcessingEvents(action));
    }
    public static void RegisterQuitRoom(PacketHandlerAction action)
    {
        handler.RegisterHandler(-16, new PacketHandler<PacketBase>.PacketProcessingEvents(action));
    }
    public static void RegisterCreateDone(PacketHandlerAction action)
    {
        handler.RegisterHandler(-15, new PacketHandler<PacketBase>.PacketProcessingEvents(action));
    }


    public static void RegisterPosHandler(PacketHandlerAction action)
    {
        handler.RegisterHandler(-10, new PacketHandler<PacketBase>.PacketProcessingEvents(action));
        return;
    }
    /// <summary>
    /// 房间改变包
    /// </summary>
    /// <param name="action"></param>
    public static void RegisterRoomPlayerChange(PacketHandlerAction action)
    {
        handler.RegisterHandler(-17, new PacketHandler<PacketBase>.PacketProcessingEvents(action));
    }
    private static void RegisterCreateItem(PacketBase packet)
    {
        var itemID = packet.ReadInt32();
        int id = 0;
        Vector3 pos = Vector3.zero;
        byte weapon = 0;
        if (itemID > 0)
        {
            id = packet.ReadInt32();
            pos = packet.ReadVector3();
            weapon = packet.ReadByte();
        }
        _itemManager.CreateOrDestoryItem(itemID, id, pos, weapon);
    }
    public static void RegisterCreateItemHanlder()
    {
        _itemManager = GameManager.Instance.assetManager.ItemManager;
        handler.RegisterHandler(-23, new PacketHandler<PacketBase>.PacketProcessingEvents(RegisterCreateItem));
    }
    public static void RegisterCreateBulletHanlder(PacketHandlerAction action)
    {
        handler.RegisterHandler(-21, new PacketHandler<PacketBase>.PacketProcessingEvents(action));
    }
    public static void RegisterAsynInfo(PacketHandlerAction action)
    {
        handler.RegisterHandler(-22, new PacketHandler<PacketBase>.PacketProcessingEvents(action));
    }
    public static void RegisterPlayerScoreChange(PacketHandlerAction action)
    {
        handler.RegisterHandler(-24, new PacketHandler<PacketBase>.PacketProcessingEvents(action));
    }
    public static void RegisterChatBox(PacketHandlerAction action)
    {
        handler.RegisterHandler(-25, new PacketHandler<PacketBase>.PacketProcessingEvents(action));
    }
    public static void RegisterPickupItem()
    {
        handler.RegisterHandler(-26, new PacketHandler<PacketBase>.PacketProcessingEvents(PlayerGetItem));
    }
    public static void ChangePlayerPos(PacketHandlerAction action)
    {
        handler.RegisterHandler(-27, new PacketHandler<PacketBase>.PacketProcessingEvents(action));
    }
    public static void ChangeWeapon(PacketHandlerAction action)
    {
        handler.RegisterHandler(-28, new PacketHandler<PacketBase>.PacketProcessingEvents(action));
    }
    public static void GetGameTime(PacketHandlerAction action)
    {
        handler.RegisterHandler(-29, new PacketHandler<PacketBase>.PacketProcessingEvents(action));
    }
    private static void PlayerGetItem(PacketBase packet)
    {
        GameManager.Instance.GetItemHandler(packet.ReadByte(), packet.ReadInt32(), packet.ReadInt32());
    }

    public static void SendSelf(PacketBase packet)
    {
        var sendPacket = new PacketBase(packet.ReadBytes());
        handler.HandlerPacket(sendPacket, sendPacket.PacketID);
    }
    /// <summary>
    /// 接收服务器的字符串信息
    /// </summary>
    public static void RegisterMessageHandler()
    {
        handler.RegisterHandler(-20, new PacketHandler<PacketBase>.PacketProcessingEvents(StringHandler));
    }
    private static void StringHandler(PacketBase packet)
    {
        GameManager.Instance.uiManager.OpenUI(UI.BaseClass.UIType.PromptPanel, packet.ReadString());
    }
    public static string MD5(this string s)
    {
        using var provider = System.Security.Cryptography.MD5.Create();
        StringBuilder builder = new StringBuilder();

        foreach (byte b in provider.ComputeHash(Encoding.UTF8.GetBytes(s)))
            builder.Append(b.ToString("x2").ToLower());

        return builder.ToString();
    }

}