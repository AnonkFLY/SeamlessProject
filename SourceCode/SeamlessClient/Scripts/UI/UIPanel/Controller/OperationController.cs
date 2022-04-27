using System.Collections;
using System.Collections.Generic;
using UI;
using UI.BaseClass;
using UnityEngine;
using UnityEngine.UI;

public class OperationController : UIController
{
    private Animator _animator;
    public override void RegisterUIManager(UIManager uiManager)
    {
        base.RegisterUIManager(uiManager);
        _animator = GetComponent<Animator>();
        var buttons = GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() => { uiManager.CloseUI(type); });
        buttons[1].onClick.AddListener(() => { uiManager.OpenUI(UIType.HelpPanel); });
        buttons[2].onClick.AddListener(GameManager.Instance.OnQuitRoom);
    }
    public override void OnClose()
    {
        _animator.Play("Close");
    }

    public override void OnOpen()
    {
        _animator.Play("Open");
    }


}
