using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomDataView : MonoBehaviour
{
    private InputField _roomName;
    private InputField _roomDescribe;
    private Slider _playerCount;
    private Text _playerCountText;
    private void Awake()
    {
        var inputFields = GetComponentsInChildren<InputField>();
        _roomName = inputFields[0];
        _roomDescribe = inputFields[1];
        _playerCount = GetComponentInChildren<Slider>();
        _playerCountText = _playerCount.GetComponentInChildren<Text>();
        _playerCount.onValueChanged.AddListener(value =>
        {
            _playerCountText.text = $"{value}/{_playerCount.maxValue}";
        });
    }
    public RoomCreateAsk GetRoomCreateAsk()
    {
        return new RoomCreateAsk(_roomName.text, (int)_playerCount.value, _roomDescribe.text);
    }
    public void Open()
    {
        if (string.IsNullOrEmpty(_roomName.text))
            _roomName.text = ClientHub.Instance.PlayerData.GetName() + "的房间";
    }
}
