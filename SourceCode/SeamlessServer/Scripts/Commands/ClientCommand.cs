using System;
using System.Collections;
using System.Collections.Generic;
using AnonCommandSystem;
using AnonSocket.AnonClient;
using UnityEngine;

public class ClientCommand : CommandStruct
{
    public UserLoginData userData;
    public Dictionary<string, ClientSocket> clientSockets = new Dictionary<string, ClientSocket>();
    public string clientName;
    public override string Execute(ParsingData data)
    {
        return null;
    }

    public override void InitCommand(CommandParser parser)
    {
        command = "Client";
        expound = "模拟客户端发包";
        parameters = new string[]
        {
            "Create [clientName:string]",       //创建一个客户端并设置名字
            "<clientName:string> Login <userName:UserLoginData>",   //登录发包
            "<clientName:string> Register <userName:UserLoginData>"//注册包
        };
    }
}
