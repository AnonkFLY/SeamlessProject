using System.Collections;
using System.Collections.Generic;
using AnonSocket.Data;
using UnityEngine;

public class RoomData
{
    public int roomID;
    public string roomName;
    public string homeowenerName;
    public int maxPlayer;
    public int currentPlayer;
    public RoomState state;
    public string describe;
    public RoomType type;
    public int gameDuration = 300;

    public RoomData(string homeowenerName = null, string roomName = null, int maxPlayer = 10, RoomType type = RoomType.TestRoom, string describe = " 无")
    {
        this.roomName = roomName ?? "ServerRoom";
        this.maxPlayer = maxPlayer;
        this.describe = describe;
        this.type = type;
        this.homeowenerName = homeowenerName ?? "Server";
    }

    public string GetString()
    {
        return JsonUtility.ToJson(this);
    }
}
public enum RoomState
{
    Waiting,
    Gaming
}
public enum RoomType
{
    TestRoom,
    Game1
}