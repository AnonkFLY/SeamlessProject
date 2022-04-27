using System.Collections.Generic;
using AnonCommandSystem;

public class ConsoleCommand
{
    private DebugInformationPanel _infoPanel;
    private CommandParser _commandParser;
    public ConsoleCommand()
    {
        _commandParser = new CommandParser();
    }
    public void RegisterCommand(CommandStruct command)
    {
        _commandParser.RegisterCommand(command);
    }
    public ReturnCommandData ParserCommand(string preInput)
    {
        return _commandParser.ParseCommand(preInput);
    }
    public HashSet<CommandStruct> GetCommandList()
    {
        return _commandParser.CommandList;
    }
    public string Execute(string input)
    {
        return _commandParser.ExecuteCommand(input);
    }
}
