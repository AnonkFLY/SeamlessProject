using System;
using System.Collections;
using System.Collections.Generic;
using AnonCommandSystem;
using UnityEngine;

public class DisplayCommand : CommandStruct
{
    public string displayMode;
    public int roomid;
    public string roomname;
    public string displayPlayerMode = "all";
    public string displayRoomMode = "all";
    public DebugInformationPanel infoPanel;
    private RoomList _roomList;
    public override string Execute(ParsingData data)
    {
        return CommandUtil.DefaultExecuteResult(data);
    }


    public override void InitCommand(CommandParser parser)
    {
        command = "Display";
        expound = "Show existing players or room";
        parameters = new string[]{
            "room", //Display all room
            "players [all|name|uid:displayPlayerMode]",//All registered players
            "onlinePlayers [all|name|uid:displayPlayerMode]",//Display all online player
            "room <int:roomID> Test",//The player in the corresponding room id
            "room <string:roomName> [all|name|player:displayRoomMode]"//The player in the corresponding room name
        };
    }
    public void DisplayData(int index)
    {
        switch (index)
        {
            case 0:
                ShowAllRoom();
                break;
            case 1:
                infoPanel.AddInformation($"All registered User:\n======{displayPlayerMode}=======");
                ShowAllPlayer(DataManager.Instance.GetAllPlayerDatas(), true);
                infoPanel.AddInformation($"===================");

                break;
            case 2:
                infoPanel.AddInformation($"All online User:\n======{displayPlayerMode}=======");
                ShowAllPlayer(DataManager.Instance.OnlinePlayers.ToArray());
                infoPanel.AddInformation($"===================");
                break;
            case 3:
                if (_roomList == null)
                    _roomList = ServerHub.Instance.RoomList;
                _roomList.GetRoom<BasicRoom>(roomid).Test();
                break;
            case 4:
                break;
        }
    }
    private void ShowAllRoom()
    {
        var rooms = _roomList.GetRooms();
        infoPanel.AddInformation($"All rooms:\n======{displayPlayerMode}=======");
        foreach (var item in rooms)
        {
            var roomData = item.Value.roomData;
            infoPanel.AddInformation($"{roomData.roomName}[{roomData.roomID}]:({roomData.currentPlayer}/{roomData.maxPlayer})");
        }
        infoPanel.AddInformation($"===================");
    }
    private void ShowAllPlayer(PlayerData[] players, bool isOnline = false)
    {
        if (players == null)
            return;
        Action<PlayerData, bool> action;
        switch (displayPlayerMode)
        {
            case "all":
                action = ShowPlayerAll;
                break;
            case "name":
                action = ShowPlayerName;
                break;
            case "uid":
                action = ShowPlayerUID;
                break;
            default:
                action = ShowPlayerAll;
                break;
        }
        foreach (var item in players)
        {
            action?.Invoke(item, isOnline);
        }
        displayPlayerMode = "all";
    }

    private void ShowPlayerName(PlayerData player, bool isOnline)
    {
        //infoPanel.AddInformation(player.playerUserName);
    }

    private void ShowPlayerUID(PlayerData player, bool isOnline)
    {
        //infoPanel.AddInformation(player.uid.ToString());
    }

    private void ShowPlayerAll(PlayerData player, bool isOnline)
    {
        string color = "#828282";
        if (isOnline)
        {
            if (DataManager.Instance.PlayerIsOnline(player.uid))
                color = "#65EA76";
        }
        infoPanel.AddInformation($"{player.playerUserName}:{player.uid}:[{player.gameName}]", color);
    }
}
