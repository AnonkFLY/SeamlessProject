using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnonCommandSystem;
using UnityEngine;

public class HelpCommand : CommandStruct
{
    public int maxShowNumber = 5;
    public int pageIndex = 1;
    public DebugInformationPanel infoPanel;
    private int currentPage = 1;
    private int _maxPage;
    private int _length;
    private CommandStruct[] _commandList;
    public override string Execute(ParsingData data)
    {
        return CommandUtil.DefaultExecuteResult(data);
    }

    public override void InitCommand(CommandParser parser)
    {
        command = "Help";
        expound = "Show all command,pageIndex 0 is next page,-1 is previous page";
        parameters = new string[] { "[int:pageIndex]" };
    }
    public void ShowCommandList(HashSet<CommandStruct> commandList)
    {
        if (_commandList == null)
        {
            _commandList = commandList.ToArray();
            _length = _commandList.Length;
            _maxPage = _length / maxShowNumber + (_length % maxShowNumber == 0 ? 0 : 1);
        }
        var viewCommandList = GetPage();
        //Debug.Log($"查看第{currentPage}页");
        infoPanel.AddInformation($"===View all commands===", "#91FF5D");
        foreach (var item in viewCommandList)
        {
            if (item != null)
            {
                infoPanel.AddInformation($"<color=white>{item.command}</color> <color=grey>-{item.expound}</color>");
                if (item.parameters != null)
                {
                    foreach (var para in item.parameters)
                    {
                        infoPanel.AddInformation($"    --{item.command} {para}", "#9F9F9F");
                    }
                }
            }
        }
        infoPanel.AddInformation($"===Page({currentPage} / {_maxPage})===", "#91FF5D");

    }

    private CommandStruct[] GetPage()
    {
        if (pageIndex <= 0)
        {
            currentPage = currentPage % _maxPage + (pageIndex == 0 ? 1 : -1);
        }
        else
        {
            currentPage = pageIndex;
        }
        currentPage = Mathf.Clamp(currentPage, 1, _maxPage);
        pageIndex = 1;
        return GetCommandRange((currentPage - 1) * maxShowNumber, maxShowNumber);
    }

    private CommandStruct[] GetCommandRange(int index, int maxShowNumber)
    {
        var result = new CommandStruct[maxShowNumber];
        if (index + maxShowNumber > _length)
        {
            maxShowNumber = _length - index;
        }
        Array.Copy(_commandList, index, result, 0, maxShowNumber);
        return result;
    }
}
