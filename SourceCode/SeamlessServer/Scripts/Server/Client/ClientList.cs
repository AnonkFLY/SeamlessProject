using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AnonSocket.AnonServer;

public class ClientList
{
    private Dictionary<Client, GameClient> _clientDict;
    public Action<GameClient> onClientLogined;
    public Action<GameClient, int> onJoinRoom;
    public ClientList()
    {
        _clientDict = new Dictionary<Client, GameClient>();
    }
    public void AddGameClient(Client client)
    {
        _clientDict.Add(client, new GameClient(client, this));
    }
    public void RemoveGameClient(Client client)
    {
        if (_clientDict.TryGetValue(client, out var getClient))
        {
            getClient.CloseClient();
            _clientDict.Remove(client);
        }
    }
    public GameClient GetGameClient(Client client)
    {
        _clientDict.TryGetValue(client, out var getGameClient);
        return getGameClient;
    }

}