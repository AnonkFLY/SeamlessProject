using System.Collections.Generic;
using UnityEngine;
public static class ScreenSizeUtil
{
    private static Dictionary<float, float> _screenSize = new Dictionary<float, float>();
    private static float hight = Screen.width / 1920;
    private static float width = Screen.height / 1080;
    public static float GetSize(float size, bool isHight = true)
    {
        _screenSize.TryGetValue(size, out var getSize);
        if (getSize != 0)
            return getSize;
        getSize = size * (isHight ? hight : width);
        UnityEngine.Debug.Log(getSize);
        if (_screenSize.ContainsKey(size))
        {
            _screenSize[size] = getSize;
        }
        else
        {
            _screenSize.Add(size, getSize);
        }
        return getSize;
    }
}