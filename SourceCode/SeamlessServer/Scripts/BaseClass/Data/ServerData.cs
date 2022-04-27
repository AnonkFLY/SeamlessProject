using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerData
{
    public string name = "ServerName";
    public ulong uid = 1000000000;
    public string GetJson()
    {
        return JsonUtility.ToJson(this, true);
    }
    public ulong GetUID()
    {
        ulong temp = uid;
        uid++;
        return temp;
    }
}
