using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreboardData : MonoBehaviour
{
    private Dictionary<ulong, PlayerScoreView> _roomScoreboard = new Dictionary<ulong, PlayerScoreView>();
    [SerializeField] private GameObject playerScoreViewPrefab;
    [SerializeField] private Transform viewRoot;
    private CanvasGroup _cavansGroup;
    private GameInfoController _gameInfo;
    private void Awake()
    {
        _cavansGroup = GetComponent<CanvasGroup>();
        GameManager.Instance.onStartGame += () =>
        {
            foreach (var item in _roomScoreboard)
            {
                item.Value.ResetValue();
            }
        };
    }
    public void SetInfoController(GameInfoController infoController)
    {
        _gameInfo = infoController;
    }
    public void Open()
    {
        _cavansGroup.interactable = true;
        _cavansGroup.blocksRaycasts = true;
        _cavansGroup.alpha = 1;
    }
    public void Close()
    {
        _cavansGroup.interactable = false;
        _cavansGroup.blocksRaycasts = false;
        _cavansGroup.alpha = 0;
    }
    public void UpdateDataOnPlayer(ulong uid, byte valueType, int value)
    {
        if (valueType == 4)
        {
            _gameInfo.SetGoldCount(value);
            return;
        }
        if (!_roomScoreboard.TryGetValue(uid, out var getValue))
            return;
        getValue.UpdateData(valueType, value);
        ReloadSort();
    }

    private void ReloadSort()
    {
        var list = _roomScoreboard.ToArray();
        var listScore = list.OrderBy(value => value.Value.score).ToArray();
        foreach (var item in listScore)
        {
            item.Value.SetUp();
        }
    }

    public void AddPlayer(ulong uid, string name)
    {
        if (_roomScoreboard.ContainsKey(uid))
            return;
        var socreView = Instantiate(playerScoreViewPrefab, viewRoot).GetComponent<PlayerScoreView>();
        socreView.SetPlayerName(name);
        _roomScoreboard.Add(uid, socreView);
        if (uid == ClientHub.Instance.PlayerData.uid)
            socreView.SetSelf();
    }
    public void RemovePlayer(ulong uid)
    {
        if (!_roomScoreboard.TryGetValue(uid, out var getValue))
            return;
        _roomScoreboard.Remove(uid);
        if (getValue.gameObject)
            Destroy(getValue.gameObject);
    }
}
