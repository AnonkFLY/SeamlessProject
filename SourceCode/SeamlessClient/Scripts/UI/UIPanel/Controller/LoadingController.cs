using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UI;
using UI.BaseClass;
using UI.BaseClass.Interface;
using UnityEngine;
using UnityEngine.UI;

public class LoadingController : UIController, IUpdatable, IStationedUpdate, IRequirementData
{
    private RectTransform _loadIcon;
    private Action ayncCallAction;
    private void Start()
    {
        _loadIcon = transform.GetChild(1).GetComponent<RectTransform>();
    }

    public override void OnClose()
    {
        UIManager.DefaultOC(canvasGroup, false);
    }

    public override void OnOpen()
    {
        UIManager.DefaultOC(canvasGroup, true);
    }

    public void UpdateView()
    {
        _loadIcon.Rotate(Vector3.down);
    }

    public void StationedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            uiManager.CloseUI(this);
        }
    }

    public bool InComingData(object data)
    {
        try
        {
            ayncCallAction = (Action)data;
            return true;
        }
        catch
        {
            return false;
        }
    }
}
