using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomView : MonoBehaviour
{
    [SerializeField] private Text roomNameText;
    [SerializeField] private Text homeowenerText;
    [SerializeField] private Text playerCountText;
    [SerializeField] private Image avatarImage;
    [SerializeField] private Button joinButton;

    [SerializeField] private Color canJoinColor;
    [SerializeField] private Color cantJoinColor;
    [SerializeField] private Color errorColor;
    private Text _buttonText;
    private RoomData _roomData;

    public RoomData RoomData { get => _roomData; }

    private void Awake()
    {
        _buttonText = joinButton.GetComponentInChildren<Text>();
        joinButton.onClick.AddListener(() =>
        {
            PacketSendContr.SendJoinRoom(_roomData);
        });
    }
    public void OpenRoom(RoomData data)
    {
        _roomData = data;
        UpdateData();
        UpdateButtonState();
    }
    private void UpdateData()
    {
        roomNameText.text = _roomData.roomName;
        homeowenerText.text = $"{_roomData.homeowenerName}\n{_roomData.describe}";
        playerCountText.text = $"人数: {_roomData.currentPlayer}/{_roomData.maxPlayer}";

        //_avatarImage 
    }
    private void UpdateButtonState()
    {
        switch (_roomData.state)
        {
            case RoomState.Waiting:
                joinButton.image.color = canJoinColor;
                _buttonText.text = "进入游戏";
                joinButton.interactable = true;
                break;
            case RoomState.Gaming:
                joinButton.image.color = cantJoinColor;
                _buttonText.text = "游戏中...";
                joinButton.interactable = false;
                break;
            default:
                joinButton.image.color = errorColor;
                _buttonText.text = "Room Error";
                joinButton.interactable = false;
                break;
        }
    }
    public void CloseRoom(Transform hideTrans)
    {
        transform.SetParent(hideTrans);
    }
}
