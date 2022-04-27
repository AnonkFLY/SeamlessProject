using System.Text;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using AnonSocket;
using System.Linq;
using System.Threading.Tasks;
using System;

public class DataManager
{
    /// <summary>
    /// 初始化-创建服务端配置文件-用户数据文件夹(GameData/PlayerDatas)
    /// 服务器配置储存数据-
    /// 
    /// 玩家数据PlayerDatas
    ///         --玩家UID
    ///         ----玩家用户名数据与密码等
    /// 获取玩家数据-根据uid
    /// 获取玩家数据-根据用户名
    /// 
    /// 创建用户并判断用户是否存在
    /// 
    /// 登录判断
    /// 
    /// LoadAll  
    /// </summary>
    private static DataManager _instance;
    public static DataManager Instance
    {
        get
        {
            return _instance;
        }
    }
    public List<PlayerData> OnlinePlayers { get => _onlinePlayers; set => _onlinePlayers = value; }

    private readonly string playerDataFolderName = "PlayerDatas";
    private readonly string serverInfo = "ServerInfo.info";
    private string _dataFolderPath;
    private string _playerDataPath;
    private string _serverInfoPath;
    private string _serverLogsPath;
    private Dictionary<string, ulong> _userUIDKeyValue;
    private ServerData _serverInfoData;
    private List<PlayerData> _onlinePlayers;
    private DebugInformationPanel _infoPanel;

    public DataManager()
    {
        _instance = this;
        InitData();
    }

    private void InitData()
    {
        _infoPanel = GameObject.FindObjectOfType<DebugInformationPanel>();
        _onlinePlayers = new List<PlayerData>();
        _userUIDKeyValue = new Dictionary<string, ulong>();


        _dataFolderPath = Path.Combine(Application.dataPath, "GameData");
        _playerDataPath = Path.Combine(_dataFolderPath, playerDataFolderName);
        _serverInfoPath = Path.Combine(_dataFolderPath, serverInfo);
        _serverLogsPath = Path.Combine(_dataFolderPath, "Logs");
        if (!Directory.Exists(_playerDataPath))
        {
            //Create data folder
            Directory.CreateDirectory(_playerDataPath);
        }
        if (!Directory.Exists(_serverLogsPath))
        {
            Directory.CreateDirectory(_serverLogsPath);
        }
        if (!File.Exists(_serverInfoPath))
        {
            this._serverInfoData = new ServerData();
            SaveServerData();
            // using (var fileStream = File.Create(_serverInfoPath))
            // {
            //     ServerData data = new ServerData();
            //     data
            //     var buff = Encoding.UTF8.GetBytes(data.GetJson());
            //     fileStream.Write(buff, 0, buff.Length);
            //     this._serverInfoData = data;
            // }
        }
        else
        {
            GetServerData();
        }
        LoadAllUserData();
    }

    private void SaveServerData()
    {
        WriteStringOnFile(_serverInfoPath, JsonUtility.ToJson(_serverInfoData));
    }
    private void GetServerData()
    {
        var jsonData = GetJsonOnFile(_serverInfoPath);
        _serverInfoData = JsonUtility.FromJson<ServerData>(jsonData);
    }

    private void LoadAllUserData()
    {
        var uidPaths = Directory.GetFiles(_playerDataPath, "*.data");
        //PlayerData[] playerDatas = new PlayerData[uidPaths.Length];
        for (int i = 0; i < uidPaths.Length; i++)
        {
            PlayerData data = JsonUtility.FromJson<PlayerData>(GetJsonOnFile(Path.Combine(uidPaths[i])));
            if (!_userUIDKeyValue.ContainsKey(data.playerUserName))
            {
                _userUIDKeyValue.Add(data.playerUserName, data.uid);
            }
            //playerDatas[i] = data;
        }
        // foreach (var item in _userUIDKeyValue)
        // {
        //     UnityEngine.Debug.Log($"用户{item.Key},UID为:{item.Value}");
        // }
    }
    public PlayerData[] GetAllPlayerDatas()
    {
        var uidPaths = Directory.GetFiles(_playerDataPath, "*.data");
        PlayerData[] playerDatas = new PlayerData[uidPaths.Length];
        for (int i = 0; i < uidPaths.Length; i++)
        {
            PlayerData data = JsonUtility.FromJson<PlayerData>(GetJsonOnFile(Path.Combine(uidPaths[i])));
            playerDatas[i] = data;
        }
        return playerDatas;
    }
    /// <summary>
    /// back uid
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public ulong RegisterPlayerData(UserLoginData player)
    {
        var data = GetUserData(player.userName);
        if (data != null)
            return 0;
        var uid = _serverInfoData.GetUID();
        WriteStringOnFile(_serverInfoPath, JsonUtility.ToJson(_serverInfoData, true));
        CreateNewUser(uid, player);
        return uid;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="passWordMD5"></param>
    /// <returns>
    /// 3 Online user
    /// 2 Wrong password
    /// 1 Username does not exist
    /// 0 other wrong
    /// </returns>
    public ulong LoginPlayer(string userName, string passWordMD5, string ip, out PlayerData data)
    {
        data = null;
        if (!_userUIDKeyValue.ContainsKey(userName))
            return 1;
        var playerData = GetUserData(userName);
        if (playerData == null)
        {
            Debug.Log("Error:data is null,userName is " + userName);
            return 0;
        }
        playerData.ip = ip;
        if (_onlinePlayers.Any<PlayerData>(data => data.uid == playerData.uid))
            return 3;
        if (passWordMD5 != playerData.playerPassWord)
            return 2;
        WriterPlayerData(playerData);
        PlayerOnline(playerData);
        data = playerData;
        return playerData.uid;
    }
    public bool PlayerIsOnline(ulong uid)
    {
        return _onlinePlayers.Any<PlayerData>(data => data.uid == uid);
    }
    public void SaveServerLog()
    {
        var now = System.DateTime.Now.ToString("yyyy-MM-dd-HH_mm_ss") + ".log";
        var logString = _infoPanel.GetServerLog();
        using (var fileStream = File.Create(Path.Combine(_serverLogsPath, now)))
        {
            var buff = Encoding.UTF8.GetBytes(logString);
            fileStream.Write(buff, 0, buff.Length);
            _infoPanel.AddInformation("log saved successfully!");
        }
    }



    public void OnPlayerSignOut(PlayerData data)
    {
        if (data == null)
            return;
        //_infoPanel.AddInformation($"用户{data.playerUserName}离线了！ip:{data.ip}", "#FF090E");
        _onlinePlayers.Remove(data);
    }
    private void PlayerOnline(PlayerData data)
    {
        _onlinePlayers.Add(data);
        //_infoPanel.AddInformation($"用户{data.playerUserName}上线了！ip:{data.ip}", "#11FF00");
    }
    private void CreateNewUser(ulong uid, UserLoginData loginData)
    {
        PlayerData playerData = new PlayerData();
        playerData.playerUserName = loginData.userName;
        playerData.playerPassWord = loginData.passWordMD5;
        playerData.uid = uid;
        WriterPlayerData(playerData);
        _userUIDKeyValue.Add(playerData.playerUserName, uid);
    }
    public void WriterPlayerData(PlayerData data)
    {
        var json = JsonUtility.ToJson(data);
        WriteStringOnFile(GetPathOnUID(data.uid), json);
    }

    private string GetJsonOnFile(string filePath)
    {
        if (!File.Exists(filePath))
            return "";
        using (var stream = File.OpenText(filePath))
        {
            return stream.ReadToEnd();
        }
    }
    private void WriteStringOnFile(string filePath, string jsonText)
    {
        using (var stream = File.Open(filePath, FileMode.Create))
        {
            var buffer = Encoding.UTF8.GetBytes(jsonText);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
    //Does the username exist
    private PlayerData GetUserData(string userName)
    {
        _userUIDKeyValue.TryGetValue(userName, out var uid);
        if (uid == 0)
            return null;
        return JsonUtility.FromJson<PlayerData>(GetJsonOnFile(GetPathOnUID(uid)));
    }
    private string GetPathOnUID(ulong uid) => Path.Combine(_playerDataPath, $"{uid.ToString()}.data");
}