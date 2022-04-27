using System.Diagnostics;
using System;
using AnonSocket;
using AnonSocket.AnonServer;
using AnonSocket.Data;

public static class PacketHandlerRegister
{
    public delegate void PacketHandlerAction(ClientData action);
    /// <summary>
    /// 注册处理
    /// </summary>
    /// <param name="client"></param>
    /// <param name="action"></param>
    public static void RegisterClientRegisterUserHandler(Client client, PacketHandlerAction action)
    {
        client.PacketHandler.RegisterHandler(PacketSendIDOnClient.RegisterID, new PacketHandler<ClientData>.PacketProcessingEvents(action));
    }
    /// <summary>
    /// 登录处理
    /// </summary>
    /// <param name="client"></param>
    /// <param name="action"></param>
    public static void ClientUserLoginHandler(Client client, PacketHandlerAction action)
    {
        client.PacketHandler.RegisterHandler(PacketSendIDOnClient.LoginID, new PacketHandler<ClientData>.PacketProcessingEvents(action));
    }
    ///  <summary>
    /// 加入房间的处理
    /// </summary>
    /// <param name="client"></param>
    /// <param name="action"></param>
    public static void JoinRoomHandler(Client client, PacketHandlerAction action)
    {
        client.PacketHandler.RegisterHandler(PacketSendIDOnClient.JoinRoom, new PacketHandler<ClientData>.PacketProcessingEvents(action));
    }
    /// <summary>
    /// 请求房间数据的处理
    /// </summary>
    /// <param name="client"></param>
    /// <param name="action"></param>
    public static void GetRoomDataAsk(Client client, PacketHandlerAction action)
    {
        client.PacketHandler.RegisterHandler(PacketSendIDOnClient.AskRoomDataID, new PacketHandler<ClientData>.PacketProcessingEvents(action));
    }
    /// <summary>
    /// 退出房间
    /// </summary>
    /// <param name="client"></param>
    /// <param name="action"></param>
    public static void QuitRoomHandler(Client client, PacketHandlerAction action)
    {
        client.PacketHandler.RegisterHandler(-13, new PacketHandler<ClientData>.PacketProcessingEvents(action));
    }
    public static void AyncPosition(Client client, PacketHandlerAction action)
    {
        client.PacketHandler.RegisterHandler(-10, new PacketHandler<ClientData>.PacketProcessingEvents(action));
    }
    public static void ClienJoinRoom(Client client, PacketHandlerAction action)
    {
        client.PacketHandler.RegisterHandler(-18, new PacketHandler<ClientData>.PacketProcessingEvents(action));
    }
    public static void ClientChangeGameName(Client client, PacketHandlerAction action)
    {
        client.PacketHandler.RegisterHandler(-19, new PacketHandler<ClientData>.PacketProcessingEvents(action));
    }
    public static void ClientPickupAsk(Client client, PacketHandlerAction action)
    {
        client.PacketHandler.RegisterHandler(-22, new PacketHandler<ClientData>.PacketProcessingEvents(action));
    }
    public static void ChatMessageHandler(Client client, PacketHandlerAction action)
    {
        client.PacketHandler.RegisterHandler(-23, new PacketHandler<ClientData>.PacketProcessingEvents(action));
    }
    public static void ClientAskStartGame(Client client, PacketHandlerAction action)
    {
        client.PacketHandler.RegisterHandler(-24, new PacketHandler<ClientData>.PacketProcessingEvents(action));
    }
    public static void ThrowItemHandler(Client client, PacketHandlerAction action)
    {
        client.PacketHandler.RegisterHandler(-25, new PacketHandler<ClientData>.PacketProcessingEvents(action));
    }
    public static void AsynWeaponHandler(Client client, PacketHandlerAction action)
    {
        client.PacketHandler.RegisterHandler(-26, new PacketHandler<ClientData>.PacketProcessingEvents(action));
    }

    public static void ClientChangePassword(Client client, PacketHandlerAction action)
    {
        client.PacketHandler.RegisterHandler(-20, new PacketHandler<ClientData>.PacketProcessingEvents(action));
    }
    public static void ClientAskSiginOut(Client client, PacketHandlerAction action)
    {
        client.PacketHandler.RegisterHandler(-3, new PacketHandler<ClientData>.PacketProcessingEvents(action));
    }
    public static void ClientRoomCreateAsk(Client client, PacketHandlerAction action)
    {
        client.PacketHandler.RegisterHandler(-8, new PacketHandler<ClientData>.PacketProcessingEvents(action));
    }
    public static void ShootBulletAsk(Client client, PacketHandlerAction action)
    {
        client.PacketHandler.RegisterHandler(-21, new PacketHandler<ClientData>.PacketProcessingEvents(action));
    }
}