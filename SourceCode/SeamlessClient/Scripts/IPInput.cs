using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Net;

public class IPInput : MonoBehaviour
{
    [SerializeField] private InputField _inputField;
    [SerializeField] private Toggle _toggle;
    GameInfo info;
    string path;

    private void Start()
    {
        path = Path.Combine(Application.dataPath, "GameInfo.info");
        DataLoad();
        _inputField.onEndEdit.AddListener(Input);
        _toggle.onValueChanged.AddListener(b => { info.isJump = b; });
    }
    private void Input(string input)
    {
        try
        {
            var ip = IPAddress.Parse(input);
            ClientHub.Instance.ServerIPAddress = input;
            info.ipInput = input;
            SaveInfo();
            LoadScene();
        }
        catch
        {
            _inputField.text = "IP格式错误";
        }
    }

    private void DataLoad()
    {

        var data = GetJsonOnFile(path);
        if (string.IsNullOrEmpty(data))
        {
            info = new GameInfo();
            info.ipInput = _inputField.text;
            SaveInfo();
        }
        else
        {
            info = JsonUtility.FromJson<GameInfo>(data);
        }
        _inputField.text = info.ipInput;
        if (info.isJump)
        {
            ClientHub.Instance.ServerIPAddress = info.ipInput;
            LoadScene();
        }
    }
    private void SaveInfo()
    {
        var json = JsonUtility.ToJson(info);
        WriteStringOnFile(path, json);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(1);
    }
    private string GetJsonOnFile(string filePath)
    {
        if (!File.Exists(filePath))
            return "";
        using (var stream = File.OpenText(filePath))
        {
            return stream.ReadToEnd();
        }
    }
    private void WriteStringOnFile(string filePath, string jsonText)
    {
        using (var stream = File.Open(filePath, FileMode.Create))
        {
            var buffer = Encoding.UTF8.GetBytes(jsonText);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
public class GameInfo
{
    public bool isJump = false;
    public string ipInput;
}
