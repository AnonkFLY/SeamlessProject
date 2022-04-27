using System;
using AnonSocket;
using AnonSocket.AnonServer;
using AnonSocket.Data;
using UnityEngine.Rendering;

public class GameClient
{
    private Client _client;
    private ClientList _list;
    private PlayerData _playerData;
    private ClientState _state;
    private RoomBase _onRoom;

    public ClientState State { get => _state; }
    public PlayerData PlayerData { get => _playerData; }
    public Client Client { get => _client; }
    public RoomBase OnRoom { get => _onRoom; }

    public Action<int> onJoinRoom;
    public Action<int> onQuitRoom;

    public GameClient(Client client, ClientList cl)
    {
        _client = client;
        _list = cl;
        ChangeClientState(ClientState.Connected);
        //InitGameClientData();
    }
    public ulong GetUID()
    {
        return _playerData.uid;
    }
    private void OnRegisterUser(ClientData data)
    {
        UserLoginData loginData = new UserLoginData();
        loginData.userName = data.packet.ReadString();
        loginData.passWordMD5 = data.packet.ReadString();
        var result = DataManager.Instance.RegisterPlayerData(loginData);

        PacketSendContr.SendRegisterHandler(_client, result);
    }
    private void OnLoginServer(ClientData data)
    {
        var packet = data.packet;
        var userName = packet.ReadString();
        var passWord = packet.ReadString();
        AnonSocketUtil.Debug($"{data.client.TcpEndPoint} [{userName}]尝试登录");
        var result = DataManager.Instance.LoginPlayer(userName, passWord, data.client.TcpEndPoint.ToString(), out var playerData);
        PacketSendContr.SendLoginHandler(_client, result);
        if (result > 10)
        {
            AnonSocketUtil.Debug($"[{userName}]登录成功");
            _playerData = playerData;
            ChangeClientState(ClientState.Logined);
            PacketSendContr.SendPlayerDataHandler(_client, playerData);
            _list?.onClientLogined?.Invoke(this);
        }
    }
    private void ChangeClientState(ClientState state)
    {
        _state = state;
        switch (state)
        {
            case ClientState.Connected:
                PacketHandlerRegister.RegisterClientRegisterUserHandler(_client, OnRegisterUser);
                PacketHandlerRegister.ClientUserLoginHandler(_client, OnLoginServer);
                _playerData = null;
                break;
            case ClientState.Logined:
                PacketHandlerRegister.JoinRoomHandler(_client, JoinRoomHandler);
                PacketHandlerRegister.ClientChangeGameName(_client, ChangeGameName);
                PacketHandlerRegister.ClientChangePassword(_client, ChangePassword);
                PacketHandlerRegister.ClientRoomCreateAsk(_client, CreateRoomAskHandler);
                break;
            case ClientState.onRoom:
                PacketHandlerRegister.QuitRoomHandler(_client, QuitRoomHandler);
                PacketHandlerRegister.ClientPickupAsk(_client, PickupItemAsk);
                PacketHandlerRegister.ChatMessageHandler(_client, ChatMessageHandler);
                PacketHandlerRegister.ShootBulletAsk(_client, ShootHandler);
                PacketHandlerRegister.ThrowItemHandler(_client, ThrowItemHandler);
                break;
        }
    }

    private void ThrowItemHandler(ClientData data)
    {
        var room = (BasicRoom)_onRoom;
        var packet = data.packet;
        var pos = packet.ReadVector3();
        var otherID = packet.ReadByte();
        var ammon = packet.ReadInt32();
        var current = packet.ReadInt32();
        room.CreateItem(1, pos, otherID, ammon, current);
    }

    private void ShootHandler(ClientData data)
    {
        if (_onRoom == null)
            return;
        var packet = data.packet;
        var uid = packet.ReadULong();
        var bulletID = packet.ReadByte();
        var pos = packet.ReadVector3();
        var dir = packet.ReadVector3();
        var player = _onRoom.GetPlayer(uid);
        ServerHub.Instance.CreateBullet(_onRoom, bulletID, pos, dir, player);
    }

    private void ChatMessageHandler(ClientData data)
    {
        if (_onRoom == null)
            return;
        _onRoom.SendChatMessage(_playerData.GetName(), data.packet.ReadString());
    }

    private void PickupItemAsk(ClientData data)
    {
        var packet = data.packet;
        OnRoom.GetPlayer(_playerData.uid).OnGetItem();
    }
    private void CreateRoomAskHandler(ClientData data)
    {
        var packet = data.packet;
        var uid = packet.ReadULong();
        var roomName = packet.ReadString();
        var playerCount = packet.ReadInt32();
        var roomDescride = packet.ReadString();
        var name = _playerData.GetName();
        BasicRoom room = new BasicRoom(name, roomName, playerCount, roomDescride);
        ServerHub.Instance.CreateRoom(room, this, true);
        PacketSendContr.SendJoinDone(_client, room.roomData.roomID);
    }
    private void JoinRoomHandler(ClientData data)
    {
        var roomID = data.packet.ReadInt32();
        var playerUID = data.packet.ReadULong();

        var room = ServerHub.Instance.RoomList.GetRoom<RoomBase>(roomID);
        var result = room.JoinPlayer(this);
        JoinRoomResult(room, result);
    }
    private void QuitRoomHandler(ClientData data)
    {
        //var roomID = data.packet.ReadInt32();
        var playerUID = data.packet.ReadULong();
        var roomID = data.packet.ReadInt32();
        QuitRoom();
    }
    private void ChangeGameName(ClientData data)
    {
        var gameName = data.packet.ReadString();
        _playerData.gameName = gameName;
        DataManager.Instance.WriterPlayerData(_playerData);
        ChangePlayerData();
    }
    private void ChangePassword(ClientData data)
    {
        var newPassWord = data.packet.ReadString();
        _playerData.playerPassWord = newPassWord;
        ChangePlayerData();
    }
    public void AddGameNumber()
    {
        _playerData.numberOfGames++;
        ChangePlayerData();
    }
    private void ChangePlayerData()
    {
        DataManager.Instance.WriterPlayerData(_playerData);
        PacketSendContr.SendPlayerDataHandler(_client, _playerData);
    }
    public void QuitRoom()
    {
        OnRoom?.QuitPlayer(this);
        _onRoom = null;
    }
    /// <summary>
    /// 加入房间
    /// </summary>
    /// <param name="roomID"></param>
    public void JoinRoomResult(RoomBase room, int result)
    {
        PacketSendContr.SendJoinResult(_client, result, room.roomData);
        if (result < 10)
            return;
        ChangeClientState(ClientState.onRoom);
        _onRoom = room;
        onJoinRoom?.Invoke(result);

        //UnityEngine.Debug.Log($"用户[{PlayerData.uid}]:{_playerData.playerUserName}进入房间{result}");
    }

    /// <summary>
    /// 退出房间
    /// </summary>
    public void QuitRoomResult(RoomBase room, int result)
    {
        // if (result < 10)
        //     return;
        _state = ClientState.Logined;
        QuitRoom();
        // //PacketSendContr.SendJoinResult(_client, result);

        // onQuitRoom?.Invoke(result);
        //UnityEngine.Debug.Log($"用户[{PlayerData.uid}]:{_playerData.playerUserName}退出房间{result}");

    }
    /// <summary>
    /// 关闭客户端
    /// </summary>
    public void CloseClient()
    {

    }


    public void SendPacket(PacketBase packet)
    {
        _client.SendMessage(packet);
    }
    public void RegisterPacketHandler(int id, Action<ClientData> handler)
    {
        _client.PacketHandler.RegisterHandler(id, new PacketHandler<ClientData>.PacketProcessingEvents(handler));
    }
}