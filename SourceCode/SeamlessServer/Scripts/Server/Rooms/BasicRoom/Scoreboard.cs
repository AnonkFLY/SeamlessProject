using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard
{
    private Dictionary<ulong, PlayerScoreboard> _scoreboardDict = new Dictionary<ulong, PlayerScoreboard>();
    public Action<ulong, byte, int> onScoreboardChange;
    private BasicRoom room;
    public Scoreboard(BasicRoom room)
    {
        this.room = room;
        room.onJoinPlayer += AddScoreboard;
        room.onQuitPlayer += RemoveScoreboard;
    }

    private void AddScoreboard(GameClient obj)
    {
        var uid = obj.GetUID();
        _scoreboardDict.Add(uid, new PlayerScoreboard(uid));
    }
    private void RemoveScoreboard(GameClient obj)
    {
        var uid = obj.GetUID();
        _scoreboardDict.Remove(uid);
    }

    public void AddPlayerScore(ulong id, int score)
    {
        var sc = GetRoomScoreboard(id);
        if (sc == null)
            return;
        sc.score += score;
        onScoreboardChange?.Invoke(id, 3, sc.score);
    }
    public void AddKillCount(ulong id, int count)
    {
        var sc = GetRoomScoreboard(id);
        if (sc == null)
            return;
        sc.killCount += count;
        onScoreboardChange?.Invoke(id, 1, sc.killCount);
    }
    public void AddDeadCount(ulong id, int count)
    {
        var sc = GetRoomScoreboard(id);
        if (sc == null)
            return;
        sc.deadCount += count;
        onScoreboardChange?.Invoke(id, 2, sc.deadCount);
    }
    public void AddGoldCount(ulong id, int count)
    {
        //UnityEngine.Debug.Log($"给玩家{id}加金币{count}");
        var sc = GetRoomScoreboard(id);
        if (sc == null)
            return;
        sc.goldCount += count;
        if (sc.goldCount >= 5)
        {
            sc.goldCount -= 5;
            AddPlayerScore(id, 1);
        }
        onScoreboardChange?.Invoke(id, 4, sc.goldCount);
    }
    public void Reset()
    {
        foreach (var item in _scoreboardDict)
        {
            item.Value.ResetValue();
        }
    }

    private PlayerScoreboard GetRoomScoreboard(ulong id)
    {
        if (_scoreboardDict.TryGetValue(id, out var getValue))
            return getValue;
        UnityEngine.Debug.Log("获取玩家失败");
        return null;
    }
}
