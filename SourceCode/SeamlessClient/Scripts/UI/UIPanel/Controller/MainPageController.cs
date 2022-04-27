using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UI.BaseClass;
using System.Diagnostics;
using System;
using AnonSocket.Data;

public class MainPageController : UIController
{
    private readonly int openHash = Animator.StringToHash("Open");
    private readonly int closeHash = Animator.StringToHash("Close");
    private RoomListContr _roomList;
    private Animator _animator;


    public override void RegisterUIManager(UIManager uiManager)
    {
        base.RegisterUIManager(uiManager);
        _roomList = GetComponentInChildren<RoomListContr>();
        _animator = GetComponent<Animator>();
    }


    public void AddRoomData(RoomData data)
    {
        _roomList.AddRoom(data);
    }
    public override void OnClose()
    {
        _animator.Play(closeHash);
    }

    public override void OnOpen()
    {
        GameManager.Instance.DelayExecute(2, _roomList.RefreshRoomAsk);
        _animator.Play(openHash);
    }
}
