using System.Collections;
using System.Collections.Generic;
using AnonSocket.Data;
using UnityEngine;

public class RoomCreateAsk
{
    public string roomName;
    public int playerCount;
    public string roomDescribe;

    public RoomCreateAsk(string roomName, int playerCount, string roomDescribe)
    {
        this.roomName = roomName;
        this.playerCount = playerCount;
        this.roomDescribe = roomDescribe;
    }
    public PacketBase GetPacketBase()
    {
        var packet = new PacketBase(-8);
        packet.Write(ClientHub.Instance.PlayerData.uid);
        packet.Write(roomName);
        packet.Write(playerCount);
        packet.Write(roomDescribe);
        return packet;
    }
}
