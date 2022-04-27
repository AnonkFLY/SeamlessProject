using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomList
{
    private Dictionary<int, RoomBase> _rooms = new Dictionary<int, RoomBase>();

    private int _roomInitID = 100;
    private int maxRoomCount = 10;
    private Transform _scenes;

    public void CreateRoom(RoomBase room)
    {
        if (_scenes == null)
            _scenes = new GameObject("Scenes").transform;
        _roomInitID++;
        _rooms.Add(_roomInitID, room);
        room.roomData.roomID = _roomInitID;
        room.LoadScene(_scenes, Vector3.up * (_roomInitID - 101) * 100);
        room.onRoomClose += (id, players) =>
        {
            _rooms.Remove(id);
        };
    }
    public T GetRoom<T>(int roomID) where T : RoomBase
    {
        _rooms.TryGetValue(roomID, out var room);
        return (T)room;
    }
    public KeyValuePair<int, RoomBase>[] GetRooms()
    {
        return _rooms.ToArray();
    }
}
