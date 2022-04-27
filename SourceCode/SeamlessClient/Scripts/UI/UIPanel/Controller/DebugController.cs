using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UI.BaseClass;
using UnityEngine;
using UnityEngine.UI;

public class DebugController : MonoBehaviour
{
    private static DebugController _instance;
    [SerializeField] private Text _debugText;

    public static DebugController Instance { get => _instance; set => _instance = value; }

    IAsyncResult result;

    private void Awake()
    {
        InitSingleton();
        _debugText = GetComponentInChildren<Text>();
    }
    public void AddDebug(object obj, int index)
    {
        switch (index)
        {
            case 0:
                result = (IAsyncResult)obj;
                break;
            default:
                break;
        }
    }
    private void Update()
    {
        //_debugText.text = $"Connect Result: {result.AsyncState} , {result.IsCompleted}";
    }
    private void InitSingleton()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
            Destroy(this.gameObject);
    }
}
