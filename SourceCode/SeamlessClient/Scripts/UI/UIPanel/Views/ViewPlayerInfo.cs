using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewPlayerInfo
{
    private Transform _texts;
    private PlayerData _playerData;
    private Text[] _textCompos;
    public ViewPlayerInfo(Transform texts)
    {
        _texts = texts;
        _textCompos = _texts.GetComponentsInChildren<Text>();
    }
    public void UpdatePlayerData(PlayerData playerData)
    {
        if (playerData == null)
            playerData = ClientHub.Instance.PlayerData;
        _playerData = playerData;
        _textCompos[0].text = $"用户名: {_playerData.playerUserName}";
        _textCompos[1].text = $"游戏场次: {_playerData.numberOfGames.ToString()}";
        var gameName = string.IsNullOrEmpty(_playerData.gameName) ? "暂无" : _playerData.gameName;
        _textCompos[2].text = $"游戏名称: {gameName}";
    }
}
