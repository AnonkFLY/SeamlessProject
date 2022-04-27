using System.Text;
using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using AnonSocket.AnonClient;
using UnityEngine;
using AnonSocket;
using AnonSocket.Data;
using UI;

public enum ClientState
{
    Connected,
    Loging,
    Logined,
    onRoom
}
public class ClientHub : MonoBehaviour
{
    //119.29.193.150
    private static ClientHub _instance;
    [SerializeField] private int _serverTCPPort;
    [SerializeField] private int _serverUDPPort;
    [SerializeField] private string _serverIPAddress;
    [SerializeField] private bool isDebug;

    private ClientState _state;
    private IPAddress _ip;
    private ClientSocket _client;
    private PlayerData _playerData;

    [SerializeField] private ProtocolTable protocolTable;

    public static ClientHub Instance { get => _instance; }

    public void SignOut()
    {
        PacketSendContr.SendSignOut();
        onSignOut?.Invoke();
        _playerData = null;
        ChangeState(ClientState.Connected);
        GameManager.Instance.SignOut();
    }

    public ClientSocket Client { get => _client; }
    public PlayerData PlayerData
    {
        get => _playerData;
        set
        {
            _playerData = value;
            onUpdatePlayerData?.Invoke(_playerData);
        }
    }

    public string ServerIPAddress { get => _serverIPAddress; set { _serverIPAddress = value; _ip = IPAddress.Parse(_serverIPAddress); } }

    public Action<PlayerData> onUpdatePlayerData;
    public Action onLogin;
    public Action onSignOut;
    public Action<int, RoomData> onJoinRoom;
    public bool isConnected;
    public Action<int> onPingDone;
    private Ping _ping;
    private void Awake()
    {
        _client = new ClientSocket(_serverTCPPort, _serverUDPPort, 32);
        PacketHandlerRegister.handler = _client.PacketHandler;
        InitSingleton();
        if (isDebug)
        {
            AnonSocketUtil.IsOpenDebug = isDebug;
            AnonSocketUtil.RegisterDebug(str => print(str));
        }
        LoginGameEventInit();
        InitEvent();
        _ip = IPAddress.Parse(_serverIPAddress);
        // var result = _client.Connect(_ip);
        // DebugController.Instance.AddDebug(result, 0);
        StartCoroutine(PingCoroutine());
    }

    private IEnumerator PingCoroutine()
    {
        var wait = new WaitForSeconds(1);
        while (true)
        {
            if (_ping == null)
                _ping = new Ping(_serverIPAddress);
            yield return wait;
            if (_ping.isDone)
            {
                onPingDone?.Invoke(_ping.time * 3);
                _ping.DestroyPing();
                _ping = null;
            }
        }
    }
    private void InitSingleton()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else if (_instance != this)
            Destroy(this.gameObject);
    }
    private void LoginGameEventInit()
    {
        _client.onConnectionFailed += ConnectFailedEvent;
        _client.onEndTCPConnect += client =>
        {
            _state = ClientState.Connected;
            isConnected = true;
            PacketHandlerRegister.RegisterMessageHandler();
        };
    }
    private void ConnectFailedEvent(ClientSocket client, Exception e)
    {
        isConnected = false;
        GameManager.Instance.uiManager.OpenUI(UI.BaseClass.UIType.ErrorInfoPanel, new ErrorPrompt("Retry", e.ToString())
        {
            confirmButtonAction = () =>
            {
                RetryServer();
                GameManager.Instance.uiManager.CloseUI(UI.BaseClass.UIType.ErrorInfoPanel);
            }
        });
    }
    public bool RetryServer()
    {
        if (isConnected)
            return isConnected;
        _client.Connect(_ip);
        return false;
    }
    public void OnDisable()
    {
        _client.Close();
    }
    public void SendPacket(PacketBase packet)
    {
        _client.SendMessage(packet);
    }
    public void ChangeState(ClientState state)
    {
        _state = state;
        switch (state)
        {
            case ClientState.Loging:
                var mainPage = GameManager.Instance.uiManager.GetUIController<MainPageController>(UI.BaseClass.UIType.MainPagePanel);
                GameManager.Instance.uiManager.OpenUI(UI.BaseClass.UIType.MainPagePanel);
                PacketHandlerRegister.RegisterRoomDataHandler(packet =>
                {
                    var roomJson = packet.ReadString();
                    var roomData = JsonUtility.FromJson<RoomData>(roomJson);
                    mainPage.AddRoomData(roomData);
                });
                ChangeState(ClientState.Logined);
                break;
            case ClientState.Logined:
                break;
            case ClientState.onRoom:
                break;
            default:
                break;
        }
    }
    private void InitEvent()
    {
        PacketHandlerRegister.RecivePlayerDataHandler(packet =>
        {
            var json = packet.ReadString();
            PlayerData = JsonUtility.FromJson<PlayerData>(json);
        });
        PacketHandlerRegister.RegisterJoinRoomResult(packet =>
        {
            int result = packet.ReadInt32();
            string json = packet.ReadString();
            RoomData room = JsonUtility.FromJson<RoomData>(json);
            onJoinRoom?.Invoke(result, room);
        });
    }
    public void GetProtocolParser(int packetID)
    {
        if (!protocolTable.clientTable.TryGetValue(-packetID, out var getValue))
        {
            UnityEngine.Debug.LogError($"接收到不存在的协议包:{packetID}");
            return;
        }
        UnityEngine.Debug.Log($"Get到数据包{packetID}--含义:{getValue.briefly}");
    }
    public void SendProtocolParser(int packetID)
    {
        if (!protocolTable.serverTable.TryGetValue(-packetID, out var getValue))
        {
            UnityEngine.Debug.LogError($"发送不存在的协议包:{packetID}");
            return;
        }
        Console.WriteLine($"Send数据包{packetID}--含义:{getValue.briefly}");
    }
    public string GetPlayerName()
    {
        if (PlayerData == null)
            return "";
        string name = _playerData.gameName;
        name = string.IsNullOrEmpty(name) ? _playerData.playerUserName : name;
        return name;
    }

}