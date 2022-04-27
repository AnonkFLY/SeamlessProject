using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create/ProtocolTable")]
public class ProtocolTable : ScriptableObject
{
    public ProtocolValue[] server;
    public ProtocolValue[] client;
    public Dictionary<int, ProtocolValue> clientTable = new Dictionary<int, ProtocolValue>();
    public Dictionary<int, ProtocolValue> serverTable = new Dictionary<int, ProtocolValue>();
    private void OnEnable()
    {
        foreach (var item in server)
        {
            serverTable.Add(item.packetID, item);
        }
        foreach (var item in client)
        {
            clientTable.Add(item.packetID, item);
        }
    }
}
[System.Serializable]
public struct ProtocolValue
{
    public int packetID;
    public Bound boundTo;
    public string briefly;
    public string describe;

}
public enum Bound
{
    Server,
    Client
}
