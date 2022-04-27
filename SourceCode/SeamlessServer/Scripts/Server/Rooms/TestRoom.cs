using System.Collections;
using System.Collections.Generic;
using AnonSocket.AnonServer;
using AnonSocket.Data;
using UnityEngine;

public class TestRoom : RoomBase
{
    public TestRoom()
    {
        roomData = new RoomData(describe: "这是一个测试");
    }


    // private Dictionary<ulong, Player> _playerPosition;
    // private GameObject _playerPrefab;
    // private float timer;
    // private bool running = false;
    // public TestRoom(GameObject playerPrefab)
    // {
    //     _playerPrefab = playerPrefab;
    //     roomData = new RoomData(describe: "这是一个测试房间");
    //     _playerPosition = new Dictionary<ulong, Player>();
    //     onJoinPlayer += JoinTestRoom;
    //     onQuitPlayer += QuitTestRoom;
    //     onRoomClose += (i, dic) =>
    //     {
    //         running = false;
    //     };
    //     timer = ServerHub.Instance.tick;

    // }
    // private void JoinTestRoom(GameClient client)
    // {
    //     ChangePlayerCount();
    //     var player = GameObject.Instantiate(_playerPrefab).GetComponent<Player>();
    //     PlayerData data = client.PlayerData;
    //     _playerPosition.Add(data.uid, player);
    //     player.PlayerData = data;
    //     player.Transform.position = Vector3.up * 2.5f;

    //     //UnityEngine.Debug.Log($"给{client.PlayerData.playerUserName}注册{player.PlayerData.playerUserName}角色");
    //     PacketHandlerRegister.AyncPosition(client.Client, clientData =>
    //      {
    //          var packet = clientData.packet;
    //          var uid = packet.ReadULong();
    //          var pos = packet.ReadVector3();
    //          var dir = packet.ReadVector3();
    //          var state = packet.ReadByte();
    //          player.Move(pos, dir, state);
    //      });
    //     PacketHandlerRegister.ClienJoinRoom(client.Client, clientData =>
    //     {
    //         SendAllClientPlayerChange(true, data, player.Transform.position);
    //         SendOtherPlayerData(data, clientData.client);
    //     });
    // }
    // private void QuitTestRoom(GameClient client)
    // {
    //     ChangePlayerCount();
    //     PlayerData data = client.PlayerData;
    //     _playerPosition.TryGetValue(data.uid, out var player);
    //     _playerPosition.Remove(data.uid);

    //     SendAllClientPlayerChange(false, data, Vector3.zero);
    //     GameObject.Destroy(player.gameObject);
    //     RoomDebug($"{data.playerUserName}-退出了游戏");
    // }
    // private IEnumerator RoomThread()
    // {
    //     running = true;
    //     RoomDebug("房间携程开启");
    //     var wait = new WaitForSeconds(timer);
    //     while (running)
    //     {
    //         var poss = GetPlayersPosAndEulerPacket();
    //         foreach (var item in playerList)
    //         {
    //             var gameClient = item.Value;
    //             PacketSendContr.AyncPosition(gameClient.Client, poss);
    //         }
    //         yield return wait;
    //     }
    //     RoomDebug("房间携程关闭");
    // }
    // private PacketBase GetPlayersPosAndEulerPacket()
    // {
    //     //8+count(4).length++count*33-(12+12+1+8)  (+2)
    //     var length = 8;
    //     length = length + 4 + _playerPosition.Count * 35;
    //     var packet = new PacketBase(-10, length);
    //     packet.Write(_playerPosition.Count);
    //     foreach (var item in _playerPosition)
    //     {
    //         //UnityEngine.Debug.Log($"广播位置数据[{item.Key}]:{item.Value.transform.position}");
    //         packet.Write(item.Key);     //(+8)
    //         item.Value.GetPosAndEluer(ref packet); //(12+12+1)
    //     }
    //     return packet;
    // }
    // private GameClient GetPlayerClient(ulong uid)
    // {
    //     playerList.TryGetValue(uid, out var getValue);
    //     return getValue;
    // }
    // ///  <summary>
    // /// 房间内有玩家进出
    // /// </summary>
    // /// <param name="join"></param>
    // /// <param name="data"></param>
    // /// <param name="pos"></param>
    // private void SendAllClientPlayerChange(bool join, PlayerData data, Vector3 pos)
    // {
    //     var packet = new PacketBase(-17);
    //     var json = data.GetJsonString();
    //     packet.Write(join);
    //     packet.Write(json);
    //     packet.Write(pos);
    //     foreach (var item in playerList)
    //     {
    //         PacketSendContr.SendInRoomPlayerData(item.Value.Client, packet);
    //     }
    // }
    // /// <summary>
    // /// 给新玩家发送所有玩家的数据
    // /// </summary>
    // /// <param name="uid">新玩家的UID</param>
    // private void SendOtherPlayerData(PlayerData data, Client client)
    // {
    //     foreach (var item in playerList)
    //     {
    //         var playerData = item.Value.PlayerData;
    //         var pos = _playerPosition[playerData.uid].Transform.position;
    //         RoomDebug($"尝试给{data.playerUserName}玩家的补充{playerData.playerUserName}数据位置:{pos}");
    //         PlayerUpdatePacket(client, true, playerData, pos);
    //     }
    // }
    // private void PlayerUpdatePacket(Client client, bool join, PlayerData data, Vector3 pos)
    // {
    //     var packet = new PacketBase(-17);
    //     var json = data.GetJsonString();
    //     packet.Write(join);
    //     packet.Write(json);
    //     packet.Write(pos);
    //     PacketSendContr.SendInRoomPlayerData(client, packet);
    // }
    // private Player GetPlayer(ulong uid)
    // {
    //     _playerPosition.TryGetValue(uid, out var getValue);
    //     return getValue;
    // }
    // private void ChangePlayerCount()
    // {
    //     if (!running && roomData.currentPlayer > 0)//如果不在运行，且人数大于0了
    //         ServerHub.Instance.StartCoroutineForServer(RoomThread());
    //     else if (roomData.currentPlayer <= 0)
    //         running = false;
    // }

}
