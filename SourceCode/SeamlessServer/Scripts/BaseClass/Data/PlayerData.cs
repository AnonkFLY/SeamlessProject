using System;
using UnityEngine;
[System.Serializable]
public class PlayerData
{
    public string playerUserName;
    public string playerPassWord;
    public ulong uid;
    public string ip;
    public int numberOfGames;
    public string gameName;
    public string GetJsonString()
    {
        string json = JsonUtility.ToJson(this);
        var obj = JsonUtility.FromJson<PlayerData>(json);
        obj.ip = "";
        obj.playerPassWord = "";
        return JsonUtility.ToJson(obj);
    }
    public string GetName()
    {
        if (string.IsNullOrEmpty(gameName))
            return playerUserName;
        return gameName;
    }
}