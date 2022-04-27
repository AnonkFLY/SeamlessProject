using System.Collections;
using System.Collections.Generic;
using AnonSocket.Data;
using UnityEngine;

public class Pickable : ItemBase
{
    public override void OnGetEvent(Player player)
    {
        //        print($"玩家{player.PlayerData.playerUserName}尝试拾取");
        var packet = new PacketBase(-26, 17);
        packet.Write(otherData);
        packet.Write(ammon);
        packet.Write(current);
        onRoom.GetPlayerClient(player.PlayerData.uid).SendPacket(packet);
        onGetEvent?.Invoke(this);
    }
}
