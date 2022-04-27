using System.Collections;
using System.Collections.Generic;
using UI;
using UI.BaseClass;
using UnityEngine;
using UnityEngine.UI;

public class HelpPanelController : UIController
{
    public override void RegisterUIManager(UIManager uiManager)
    {
        base.RegisterUIManager(uiManager);
        GetComponentInChildren<Button>().onClick.AddListener(() => { uiManager.CloseUI(type); });
    }
    public override void OnClose()
    {
        UIManager.DefaultOC(canvasGroup, false, 0.8f);
    }

    public override void OnOpen()
    {
        UIManager.DefaultOC(canvasGroup, true, 0.8f);
    }
}
