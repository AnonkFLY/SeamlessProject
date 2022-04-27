using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UI;
using UI.BaseClass;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomOpeation : UIController
{
    private CreateRoomDataView _data;
    private Button _createRoom;
    private Button _cancel;
    public override void RegisterUIManager(UIManager uiManager)
    {
        base.RegisterUIManager(uiManager);
        _data = GetComponentInChildren<CreateRoomDataView>();
        var buttons = GetComponentsInChildren<Button>();
        _createRoom = buttons[0];
        _cancel = buttons[1];
        _createRoom.onClick.AddListener(CreateRoom);
        _cancel.onClick.AddListener(Cancel);
        PacketHandlerRegister.RegisterCreateDone(data =>
        {
            GameManager.Instance.isHomowenr = true;
            var roomID = data.ReadInt32();
            PacketSendContr.SendJoinRoom(roomID);
        });
    }
    public override void OnClose()
    {
        UIManager.DefaultOC(canvasGroup, false, 0.8f);
    }

    public override void OnOpen()
    {
        UIManager.DefaultOC(canvasGroup, true, 0.4f);
        _data.Open();
    }
    private void Cancel()
    {
        uiManager.CloseUI(type);
    }
    private void CreateRoom()
    {
        //Send create room packet
        var packet = _data.GetRoomCreateAsk();

        PacketSendContr.SendCreateRoom(packet.GetPacketBase());
        uiManager.CloseUI(type);
    }
}
