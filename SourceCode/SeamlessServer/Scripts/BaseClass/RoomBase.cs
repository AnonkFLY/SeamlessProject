using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using AnonSocket.AnonServer;
using AnonSocket.Data;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public abstract class RoomBase
{
    // public int roomID;
    // public string roomName;
    // public string homeowenerName;
    // public int maxPlayer;
    // public int currentPlayer;
    // public RoomState state;
    // public string describe;
    // public RoomType type;

    public RoomData roomData;
    private GameClient homeowner;
    public GameObject Scenes;
    public Action<int> onGaming;
    //protected int sceneIndex = 0;

    public Dictionary<ulong, GameClient> playerList = new Dictionary<ulong, GameClient>();
    public Dictionary<ulong, Player> playerPosition = new Dictionary<ulong, Player>();
    protected DebugInformationPanel infoPanel;

    public Action<GameClient> onJoinPlayer;
    public Action<GameClient> onQuitPlayer;

    public Action<int, Dictionary<ulong, GameClient>> onRoomClose;
    public Action onStartGame;
    public Action onOverGame;
    public Action<Player, Player> onPlayerDead;

    private bool running = false;
    private GameObject _playerPrefab;
    private float _timer;
    public bool _isOpenDamage = false;
    protected Transform[] birthPoint;
    protected GoldUpdate[] goldPoint;
    protected WeaponUpdate[] weaponPoint;
    private Coroutine game;
    public Action<Client> onPlayerJoinDone;
    public GameClient Homeowner
    {
        get => homeowner;
        set
        {
            homeowner = value;
            if (homeowner != null)
                PacketHandlerRegister.ClientAskStartGame(homeowner.Client, OnStartGameHandler);
        }
    }
    private void OnStartGameHandler(ClientData clientData)
    {
        if (roomData.state == RoomState.Gaming)
            return;
        OnStartGame();
    }
    public void CreateBullet(byte id, Vector3 pos, Vector3 addForce)
    {
        var packet = new PacketBase(-21, 33);
        //客户端发射子弹(byte:BulletType,pos,addForce)
        packet.Write(id);
        packet.Write(pos);
        packet.Write(addForce);
        BroadCast(packet);
    }
    public void BroadCast(PacketBase packet)
    {
        foreach (var player in playerList)
        {
            player.Value.SendPacket(packet);
        }
        //ServerHub.Instance.StartCoroutine(BroadCastCoutine(packet));
    }
    // private IEnumerator BroadCastCoutine(PacketBase packet)
    // {
    //     var wait = new WaitForEndOfFrame();
    //     foreach (var player in playerList)
    //     {
    //         player.Value.SendPacket(packet);
    //         yield return wait;
    //     }
    // }
    private void OnOverGame()
    {
        if (!running)
            return;
        UnityEngine.Debug.Log("OverGame");
        onOverGame?.Invoke();
        roomData.state = RoomState.Waiting;
        _isOpenDamage = false;
        foreach (var item in playerList)
        {
            item.Value.AddGameNumber();
        }
    }

    // 玩家加入房间
    // 玩家退出房间
    // 创建房间
    // 关闭房间
    private void OnStartGame()
    {
        roomData.state = RoomState.Gaming;
        UnityEngine.Debug.Log("Start");
        game = ServerHub.Instance.StartCoroutine(StartGame());
    }
    private IEnumerator StartGame()
    {
        var wait = new WaitForSeconds(1);

        int time = -6;
        while (time < -1)
        {
            time++;
            SendTime(time);
            yield return wait;
        }
        onStartGame?.Invoke();
        _isOpenDamage = true;
        time = roomData.gameDuration;

        while (time > 0)
        {
            yield return wait;
            time--;
            onGaming?.Invoke(time);
            SendTime(time);
            if (!running)
                break;
        }
        OnOverGame();
    }
    private void SendTime(int time)
    {
        PacketBase packetTime = new PacketBase(-29, 12);
        packetTime.Write(time);
        BroadCast(packetTime);
    }
    public virtual int JoinPlayer(GameClient player)
    {
        if (roomData.state == RoomState.Gaming)
        {
            SendPlayer(player, "房间已开始游戏");
            return 2;
        }
        if (roomData.maxPlayer <= roomData.currentPlayer)
        {
            RoomDebug("房间人数已满");
            SendPlayer(player, "房间已满");
            return 0;
        }
        var uid = player.PlayerData.uid;
        if (playerList.ContainsKey(uid))
        {
            SendPlayer(player, $"Error:玩家{player.PlayerData.playerUserName}已存在与该房间");
            RoomDebug($"Error:玩家{player.PlayerData.playerUserName}已存在与该房间");
            return 1;
        }
        RoomDebug($"玩家{player.PlayerData.playerUserName}加入房间");
        playerList.Add(uid, player);
        roomData.currentPlayer++;

        onJoinPlayer?.Invoke(player);
        return roomData.roomID;
    }

    public virtual void QuitPlayer(GameClient player)
    {
        var uid = player.PlayerData.uid;
        var result = playerList.Remove(uid);
        if (result)
        {
            roomData.currentPlayer--;

            onQuitPlayer?.Invoke(player);
            player.QuitRoomResult(this, roomData.roomID);
            RoomDebug($"玩家{player.PlayerData.playerUserName}退出房间");
            if (homeowner == player)
            {
                CloseRoom();
            }
        }
        else
        {
            RoomDebug($"错误,无法退出玩家{player.PlayerData.playerUserName}");
        }
    }
    //no done
    public void CloseRoom()
    {
        //RoomDebug("尝试关闭房间");
        onRoomClose?.Invoke(roomData.roomID, playerList);
        if (homeowner != null)
        {
            QuitPlayer(homeowner);
        }
        foreach (var item in playerList)
        {
            PacketSendContr.QuitRoomPlayer(item.Value.Client);
        }
        GameObject.Destroy(Scenes);
    }
    public void RegiseterInfoDebug(DebugInformationPanel panel)
    {
        infoPanel = panel;
    }
    protected void RoomDebug(string info)
    {
        //infoPanel?.AddInformation($"Room[{roomData.roomID}]:{info}");
    }

    private void JoinTestRoom(GameClient client)
    {
        ChangePlayerCount();
        var player = GameObject.Instantiate(_playerPrefab, Scenes.transform).GetComponent<Player>();
        PlayerData data = client.PlayerData;
        playerPosition.Add(data.uid, player);
        player.PlayerData = data;
        player.Transform.localPosition = Vector3.up * 2.5f;
        InitPlayerEvent(player, client);
        player.Info.onInfoChange += (h, a, maxH, maxA) =>
        {
            var packet = new PacketBase(-22, 32);
            packet.Write(data.uid);
            packet.Write(h);
            packet.Write(a);
            packet.Write(maxH);
            packet.Write(maxA);
            BroadCast(packet);
        };
        //UnityEngine.Debug.Log($"给{client.PlayerData.playerUserName}注册{player.PlayerData.playerUserName}角色");
        PacketHandlerRegister.AyncPosition(client.Client, clientData =>
         {
             var packet = clientData.packet;
             var uid = packet.ReadULong();
             var pos = packet.ReadVector3();
             var dir = packet.ReadVector3();
             var state = packet.ReadByte();
             player.Move(pos, dir, state);
         });
        PacketHandlerRegister.AsynWeaponHandler(client.Client, clientData =>
        {
            byte weapon = clientData.packet.ReadByte();
            player.AysnWeapon(weapon);
            var uid = player.PlayerData.uid;
            var packet = new PacketBase(-28, 17);
            packet.Write(uid);
            packet.Write(weapon);
            BroadCast(packet);
        });
        PacketHandlerRegister.ClienJoinRoom(client.Client, clientData =>
        {
            SendAllClientPlayerChange(true, data, player.Transform.localPosition);
            SendOtherPlayerData(data, clientData.client);
        });
    }


    private void QuitTestRoom(GameClient client)
    {
        ChangePlayerCount();
        PlayerData data = client.PlayerData;

        if (playerPosition.TryGetValue(data.uid, out var player))
        {
            try
            {
                ServerHub.Instance.OnMainThreadExecute(() =>
                {
                    GameObject.Destroy(player.gameObject);
                });
                playerPosition.Remove(data.uid);
                SendAllClientPlayerChange(false, data, Vector3.zero);
                RoomDebug($"{data.playerUserName}-退出了游戏");
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e);

            }
        }
    }
    private IEnumerator RoomThread()
    {
        running = true;
        RoomDebug("房间携程开启");
        var wait = new WaitForSeconds(_timer);
        while (running)
        {
            var poss = GetPlayersPosAndEulerPacket();
            foreach (var item in playerList)
            {
                var gameClient = item.Value;
                PacketSendContr.AyncPosition(gameClient.Client, poss);
            }
            yield return wait;
        }
        RoomDebug("房间携程关闭");
    }
    private PacketBase GetPlayersPosAndEulerPacket()
    {
        //8+count(4).length++count*33-(12+12+1+8)  (+2)
        var length = 8;
        length = length + 4 + playerPosition.Count * 35;
        var packet = new PacketBase(-10, length);
        packet.Write(playerPosition.Count);
        foreach (var item in playerPosition)
        {
            //UnityEngine.Debug.Log($"广播位置数据[{item.Key}]:{item.Value.transform.position}");
            packet.Write(item.Key);     //(+8)
            item.Value.GetPosAndEluer(ref packet); //(12+12+1)
        }
        return packet;
    }
    public GameClient GetPlayerClient(ulong uid)
    {
        playerList.TryGetValue(uid, out var getValue);
        return getValue;
    }
    public Player GetPlayer(ulong uid)
    {
        playerPosition.TryGetValue(uid, out var getValue);
        return getValue;
    }
    ///  <summary>
    /// 房间内有玩家进出
    /// </summary>
    /// <param name="join"></param>
    /// <param name="data"></param>
    /// <param name="pos"></param>
    private void SendAllClientPlayerChange(bool join, PlayerData data, Vector3 pos)
    {
        var packet = new PacketBase(-17);
        var json = data.GetJsonString();
        packet.Write(join);
        packet.Write(json);
        packet.Write(pos);
        foreach (var item in playerList)
        {
            PacketSendContr.SendInRoomPlayerData(item.Value.Client, packet);
        }
    }
    /// <summary>
    /// 给新玩家发送所有玩家的数据
    /// </summary>
    /// <param name="uid">新玩家的UID</param>
    private void SendOtherPlayerData(PlayerData data, Client client)
    {
        foreach (var item in playerList)
        {
            var playerData = item.Value.PlayerData;
            var pos = playerPosition[playerData.uid].Transform.localPosition;
            RoomDebug($"尝试给{data.playerUserName}玩家的补充{playerData.playerUserName}数据位置:{pos}");
            PlayerUpdatePacket(client, true, playerData, pos);
        }
        onPlayerJoinDone?.Invoke(client);

    }
    private void PlayerUpdatePacket(Client client, bool join, PlayerData data, Vector3 pos)
    {
        var packet = new PacketBase(-17);
        var json = data.GetJsonString();
        packet.Write(join);
        packet.Write(json);
        packet.Write(pos);
        PacketSendContr.SendInRoomPlayerData(client, packet);
    }

    private void ChangePlayerCount()
    {
        if (!running && roomData.currentPlayer > 0)//如果不在运行，且人数大于0了
            ServerHub.Instance.StartCoroutineForServer(RoomThread());
        else if (roomData.currentPlayer <= 0)
            running = false;

    }
    public virtual void InitRoom(GameObject playerPrefab)
    {
        _timer = ServerHub.Instance.tick;
        _playerPrefab = playerPrefab;
        onJoinPlayer += JoinTestRoom;
        onQuitPlayer += QuitTestRoom;
    }
    public GameObject LoadScene(Transform transform, Vector3 pos)
    {
        Scenes = GameObject.Instantiate(ServerHub.Instance.GetScenes((int)roomData.type), transform);
        Scenes.transform.localPosition = pos;
        birthPoint = Scenes.transform.Find("BirthPoints").GetComponentsInChildren<Transform>();
        weaponPoint = Scenes.transform.Find("WeaponPoints").GetComponentsInChildren<WeaponUpdate>();
        goldPoint = Scenes.transform.Find("GoldPoints").GetComponentsInChildren<GoldUpdate>();
        return Scenes;
    }
    private void SendPlayer(GameClient client, string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        var packet = new PacketBase(20, bytes.Length + 4);
        packet.Write(bytes.Length);
        packet.Write(bytes);
        client.SendPacket(packet);
    }
    public void SendPacket(ulong uid, PacketBase packet)
    {
        if (!playerList.TryGetValue(uid, out var getValue))
            return;
        getValue.Client.SendMessage(packet);
    }
    public void SendChatMessage(string name, string message)
    {
        var packet = new PacketBase(-25);
        packet.Write(name);
        packet.Write(message);
        BroadCast(packet);
    }
    public void SendSystemMessage(string message, string color = null)
    {
        if (color != null)
        {
            message = $"<color={color}>{message}</color>";
        }
        SendChatMessage("AnonkServer", message);
    }
    private void InitPlayerEvent(Player player, GameClient client)
    {
        player.onDead += (player, playerOrgin) =>
        {
            if (playerOrgin != null)
                SendSystemMessage($"{playerOrgin?.PlayerData.GetName()} 击败了 {player.PlayerData.GetName()}");
            else
                SendSystemMessage($"{player.PlayerData.GetName()} 狗带了");
            onPlayerDead?.Invoke(player, playerOrgin);
        };
        player.onRenascence += player =>
        {
            Vector3 pos;
            int r;
            if (roomData.state == RoomState.Waiting)
                r = 0;
            else
                r = UnityEngine.Random.Range(1, birthPoint.Length); ;
            pos = birthPoint[r].localPosition + Vector3.up * 4.5f;
            SetPlayerPos(player.PlayerData.uid, pos);
            player.Info.Reset();
        };
    }
    public void SetPlayerPos(ulong uid, Vector3 pos)
    {
        var packet = new PacketBase(-27, 20);
        packet.Write(pos);
        GetPlayerClient(uid).SendPacket(packet);
    }
}
