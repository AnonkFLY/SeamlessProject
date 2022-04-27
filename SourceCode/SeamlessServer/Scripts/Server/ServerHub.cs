using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnonSocket.AnonServer;
using AnonSocket;
using System;
using AnonSocket.Data;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class ServerHub : MonoBehaviour
{
    private static ServerHub _instance;
    private ServerSocket _server;
    private bool isOpen = true;

    private DataManager _dataManager;
    private ClientList _clientList;
    private RoomList _roomList;
    private DebugInformationPanel _infoPanel;

    [SerializeField] private int _maxCnt;
    [SerializeField] private int _tdpProt;
    [SerializeField] private int _udpProt;
    [SerializeField] private GameObject playerPrefab;

    public bool isDebug = false;

    public ProtocolTable protocolTable;

    private List<Action> _mainThreadAction;
    private List<Client> _sendRoomData;

    public ServerSocket Server { get => _server; }
    public static ServerHub Instance { get => _instance; }
    public DebugInformationPanel InfoPanel { get => _infoPanel; }
    public RoomList RoomList { get => _roomList; }
    public ClientList ClientList { get => _clientList; }
    [HideInInspector]
    public float tick;
    [SerializeField]
    private int theTick = 32;
    private int index = 0;
    [SerializeField] private GameObject[] _scenes;
    [SerializeField] private GameObject[] _items;

    [SerializeField] private BulletManager _bulletManager;
    private Transform _cameraTrans;
    private float speed = 5;
    private void Awake()
    {
        tick = 1f / theTick;
        _cameraTrans = Camera.main.transform;
        InitSingleton();
        _mainThreadAction = new List<Action>();
        _sendRoomData = new List<Client>();
        _server = new ServerSocket(_maxCnt, _tdpProt, _udpProt, 32);
        _clientList = new ClientList();
        _dataManager = new DataManager();
        _roomList = new RoomList();
        _infoPanel = FindObjectOfType<DebugInformationPanel>();
        if (isDebug)
        {
            AnonSocketUtil.IsOpenDebug = isDebug;
            AnonSocketUtil.RegisterDebug(str => _infoPanel.AddInformation(str));
        }
        _server.Open();
        InitServerHandler();
        Task.Run(StartSendRoomData);
    }

    void Start()
    {
        CreateTestRoom();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsEsc();
        }

        lock (_mainThreadAction)
        {
            try
            {

                if (_mainThreadAction.Count > 0)
                {

                    for (int i = 0; i < _mainThreadAction.Count; i++)
                    {
                        try
                        {
                            _mainThreadAction[i]?.Invoke();
                        }
                        catch (Exception e)
                        {
                            print($"Error:" + e);
                            continue;
                        }
                    }
                    _mainThreadAction.Clear();
                }
            }
            catch (Exception e)
            {
                print("主线程委托队列Error:" + e);
                _mainThreadAction.Clear();
            }
        }
        if (isOpen)
            return;
        InputHandler();
    }

    private void InputHandler()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = 10;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = 5;
        }
        var dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (dir == Vector2.zero)
            return;
        var overDir = dir.x * _cameraTrans.right + dir.y * _cameraTrans.forward;
        overDir.Normalize();
        _cameraTrans.Translate(speed * Time.deltaTime * overDir, Space.World);
        var dirW = speed * Time.deltaTime * Vector3.up;
        if (Input.GetKey(KeyCode.Space))
            _cameraTrans.position += dirW;
        if (Input.GetKey(KeyCode.LeftControl))
            _cameraTrans.position -= dirW;
    }

    private void IsEsc()
    {
        isOpen = !isOpen;
        _infoPanel._canvasGroup.alpha = isOpen ? 1 : 0;
        _infoPanel._canvasGroup.interactable = isOpen;
        _infoPanel._canvasGroup.blocksRaycasts = isOpen;
    }

    public void OnMainThreadExecute(Action action)
    {
        _mainThreadAction.Add(action);
    }
    private void InitServerHandler()
    {
        _server.onClientConnet += OnClientConnect;
        _server.onClientDisconnect += OnClientDisconnected;

        _clientList.onClientLogined += SendRoomData;
        _clientList.onClientLogined += gameClient =>
        {
            PacketHandlerRegister.GetRoomDataAsk(gameClient.Client, data =>
            {
                SendRoomData(gameClient);
            });
            PacketHandlerRegister.ClientAskSiginOut(gameClient.Client, data =>
            {
                SignOutPlayer(gameClient);
            });
        };
    }


    private void SendRoomData(GameClient loginClient)
    {
        //_infoPanel.AddInformation("try send room data to " + loginClient.PlayerData.playerUserName);

        lock (_sendRoomData)
        {
            _sendRoomData.Add(loginClient.Client);
        }
    }
    public void SendClientRoomsData(Client client)
    {
        var rooms = _roomList.GetRooms();
        foreach (var item in rooms)
            SendClientRoomData(client, item);
    }
    private void SendClientRoomData(Client client, KeyValuePair<int, RoomBase> room)
    {
        var packet = new PacketBase(-7);
        //_infoPanel.AddInformation($"发送房间数据...", "red");
        var roomData = room.Value;
        packet.Write(roomData.roomData.GetString());
        PacketSendContr.SendRoomData(client, packet);
    }
    private void OnClientDisconnected(Client client, int index)
    {

        var gamePlayer = _clientList.GetGameClient(client);
        // _infoPanel.AddInformation($"{client.TcpEndPoint} 断开服务器...", "red");
        // _infoPanel.AddInformation($"{gamePlayer} 断开服务器...", "red");
        if (gamePlayer == null)
            return;
        SignOutPlayer(gamePlayer);
        gamePlayer.QuitRoom();
        RemeveClientConnect(client);
    }
    /// <summary>
    /// 退出一个客户端的登录
    /// </summary>
    /// <param name="gamePlayer"></param>
    public void SignOutPlayer(GameClient gamePlayer)
    {
        _dataManager.OnPlayerSignOut(gamePlayer.PlayerData);
    }
    public void RemeveClientConnect(Client client)
    {
        _clientList.RemoveGameClient(client);
    }
    private void OnClientConnect(Client client, int index)
    {
        //_infoPanel.AddInformation($"{client.TcpEndPoint} 连接至服务器...");
        _clientList.AddGameClient(client);
    }

    private void OnDisable()
    {
        _server?.Close();
        _server = null;
    }
    private void CreateTestRoom()
    {
        var basic = new BasicRoom("Server", "Server", 11, "ServerTest", RoomType.TestRoom);
        CreateRoom(basic, null, true);
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
    private void StartSendRoomData()
    {
        while (true)
        {
            if (_sendRoomData.Count < 0)
                continue;
            lock (_sendRoomData)
            {
                foreach (var item in _sendRoomData)
                {
                    SendClientRoomsData(item);
                }
                _sendRoomData.Clear();
            }
            Thread.Sleep(100);
        }
    }
    public Coroutine StartCoroutineForServer(IEnumerator coroutine)
    {
        return StartCoroutine(coroutine);
    }

    public void GetProtocolParser(int packetID)
    {
        if (!protocolTable.serverTable.TryGetValue(-packetID, out var getValue))
        {
            UnityEngine.Debug.LogError($"接收到不存在的协议包:{packetID}");
            return;
        }
        _infoPanel.AddInformation($"Get到数据包{packetID}--含义:{getValue.briefly}", "#494949");
    }
    public void SendProtocolParser(int packetID)
    {
        if (!protocolTable.clientTable.TryGetValue(-packetID, out var getValue))
        {
            UnityEngine.Debug.LogError($"发送不存在的协议包:{packetID}");
            return;
        }
        _infoPanel.AddInformation($"Send数据包{packetID}--含义:{getValue.briefly}", "#494949");
    }
    public void CreateRoom(RoomBase roomBase, GameClient homeowener, bool isOpenDebug = false)
    {
        roomBase.InitRoom(playerPrefab);
        roomBase.Homeowner = homeowener;
        _roomList.CreateRoom(roomBase);
        if (!isOpenDebug)
            return;
        roomBase.RegiseterInfoDebug(_infoPanel);
    }
    public GameObject GetScenes(int index)
    {

        return _scenes[index];
    }
    public GameObject GetItems(int index)
    {
        return _items[index];
    }
    public void CreateBullet(RoomBase room, byte type, Vector3 pos, Vector3 dir, Player player)
    {
        var bullet = _bulletManager.GetBulletInstance(type);
        bullet.Init(room, pos, dir, player);
    }
}
public enum ClientState
{
    Connected,
    Logined,
    onRoom
}
