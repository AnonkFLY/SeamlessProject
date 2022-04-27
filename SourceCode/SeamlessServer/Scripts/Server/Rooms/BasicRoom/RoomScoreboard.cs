using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerScoreboard
{
    public ulong uid;
    public int killCount;
    public int deadCount;
    public int goldCount;
    public int score;

    public PlayerScoreboard(ulong uid)
    {
        this.uid = uid;
    }
    public void ResetValue()
    {
        killCount = 0;
        deadCount = 0;
        goldCount = 0;
        score = 0;
    }
}
