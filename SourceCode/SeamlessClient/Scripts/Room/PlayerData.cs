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
    public string GetName()
    {
        if (string.IsNullOrEmpty(gameName))
            return playerUserName;
        return gameName;
    }
}