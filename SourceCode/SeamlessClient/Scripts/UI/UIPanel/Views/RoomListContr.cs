using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RoomListContr : MonoBehaviour
{
    [SerializeField] private Transform contentTrans;
    [SerializeField] private Transform bufferPoolTrans;
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button refreshRoomButton;
    private Dictionary<int, RoomView> _roomList = new Dictionary<int, RoomView>();
    private List<RoomView> _bufferPool = new List<RoomView>();
    private void Start()
    {
        createRoomButton.onClick.AddListener(CreateRoomAsk);
        refreshRoomButton.onClick.AddListener(RefreshRoomAsk);
    }

    public void RefreshRoomAsk()
    {
        CloseAllRoom();
        PacketSendContr.AskRoomsData();
    }

    private void CreateRoomAsk()
    {
        //GameManager.Instance.uiManager.OpenUI(UI.BaseClass.UIType.PromptPanel, "还不可以创建房间哦");
        GameManager.Instance.uiManager.OpenUI(UI.BaseClass.UIType.CreateRoomPanel);
    }

    public void AddRoom(RoomData data)
    {
        if (_roomList.TryGetValue(data.roomID, out var getValue))
        {
            getValue.OpenRoom(data);
            return;
        }
        var room = GetRoomInstance();
        room.OpenRoom(data);
        _roomList.Add(data.roomID, room);
    }
    public void Close(int roomID)
    {
        if (!_roomList.TryGetValue(roomID, out var getRoom))
            return;
        //print($"关闭房间{roomID}");
        _roomList.Remove(roomID);
        getRoom.CloseRoom(bufferPoolTrans);
        _bufferPool.Remove(getRoom);
    }
    public RoomData GetRoomData(int id)
    {
        _roomList.TryGetValue(id, out var getValue);
        return getValue.RoomData;
    }
    public void CloseAllRoom()
    {
        var list = _roomList.ToArray();
        foreach (var item in list)
        {
            Close(item.Key);
        }
    }
    private RoomView GetRoomInstance()
    {
        if (_bufferPool.Count <= 0)
            return InstantieRoomObj();
        var index = _bufferPool.Count - 1;
        var result = _bufferPool[index];
        _bufferPool.RemoveAt(index);
        return result;
    }
    private RoomView InstantieRoomObj()
    {
        return Instantiate(roomPrefab, contentTrans).GetComponent<RoomView>();
    }

}
