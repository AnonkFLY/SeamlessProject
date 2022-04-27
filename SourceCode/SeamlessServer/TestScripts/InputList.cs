using System.Collections.Generic;

public class InputList
{
    private List<string> _inputList = new List<string>();
    private int _index = 0;
    public string GetUp()
    {
        if (_inputList.Count < 1)
            return "";
        _index--;
        if (_index == -1)
        {
            _index = 0;
        }
        return _inputList[_index];
    }
    public string GetDown()
    {
        if (_inputList.Count < 1)
            return "";
        _index++;
        if (_index >= _inputList.Count)
        {
            _index = _inputList.Count - 1;
        }
        return _inputList[_index];
    }
    public void AddInput(string input)
    {
        _inputList.Add(input);
        _index++;
    }
    public void ResetIndex()
    {
        _index = _inputList.Count;
    }
}