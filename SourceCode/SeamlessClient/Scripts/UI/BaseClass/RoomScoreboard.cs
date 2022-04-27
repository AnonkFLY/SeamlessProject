using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomScoreboard
{
    public string gameName;
    public ulong uid;
    public int killCount;
    public int deadCount;
    public int score;

    public RoomScoreboard(ulong uid)
    {
        this.uid = uid;
    }
}
