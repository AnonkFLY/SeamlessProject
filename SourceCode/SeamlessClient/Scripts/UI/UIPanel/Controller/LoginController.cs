using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UI;
using UI.BaseClass;
using UI.BaseClass.Interface;
using UI.Panels;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class LoginController : UIController
{
    private LoginUIView _loginView;
    private float _orginPosY;
    [SerializeField] private float timer = 0.5f;
    public override void RegisterUIManager(UIManager uiManager)
    {
        base.RegisterUIManager(uiManager);
        _loginView = new LoginUIView();
        _loginView.InitView(transform);
        _orginPosY = rectTrans.localPosition.y;
    }

    public override void OnOpen()
    {
        print(rectTrans);
        rectTrans.DOLocalMoveY(0, 0.4f);
        UIManager.DefaultOC(canvasGroup, true);
        _loginView.onTryLogin += OnLoginBack;
    }

    private void OnLoginBack(ulong uid)
    {
        if (uid > 10)
        {
            uiManager.CloseUI(type);
            //uiManager.OpenUI(UIType.MainPagePanel);
        }
    }

    public override void OnClose()
    {
        rectTrans.DOLocalMoveY(_orginPosY, timer).onComplete += () =>
        {
            UIManager.DefaultOC(canvasGroup, false);
        };
        _loginView.onTryLogin -= OnLoginBack;
    }

}

public struct LoginData
{
    public string userName;
    public string passWord;
    public string passWordAgain;

    public LoginData(string userName, string passWord, string passWordAgain)
    {
        this.userName = userName;
        this.passWord = passWord;
        this.passWordAgain = passWordAgain;
    }
}
