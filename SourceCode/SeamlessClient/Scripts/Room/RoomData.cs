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

    public RoomData(int roomID, string roomName, int maxPlayer, RoomType type, string describe = " 无")
    {
        this.roomID = roomID;
        this.roomName = roomName;
        this.maxPlayer = maxPlayer;
        this.describe = describe;
        this.type = type;
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