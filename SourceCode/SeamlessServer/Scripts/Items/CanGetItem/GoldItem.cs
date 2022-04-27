using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldItem : ItemBase
{
    public override void OnGetEvent(Player player)
    {
        var value = otherData + 1;
        if (value == 2)
            value++;
        onRoom.scoreboard.AddGoldCount(player.PlayerData.uid, value);
        player.Info.AddHealth(value);
        onGetEvent?.Invoke(this);
    }
}
