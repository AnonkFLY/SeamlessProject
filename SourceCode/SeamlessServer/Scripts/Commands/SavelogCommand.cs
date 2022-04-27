using System.Collections;
using System.Collections.Generic;
using AnonCommandSystem;
using UnityEngine;

public class SavelogCommand : CommandStruct
{

    public override string Execute(ParsingData data)
    {
        DataManager.Instance.SaveServerLog();
        return CommandUtil.DefaultExecuteResult(data);
    }

    public override void InitCommand(CommandParser parser)
    {
        command = "SaveLog";
        expound = "Save the current server running log";
    }
}
