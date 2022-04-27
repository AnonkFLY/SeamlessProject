using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UI;
using UI.BaseClass;
using UI.BaseClass.Interface;
using UnityEngine;
using UnityEngine.UI;

public class StartGameController : UIController, IUpdatable
{
    private Text _getAnyKey;
    private Tween _textAnimation;
    [SerializeField]
    private float textTimer = 1;
    private bool _stop;
    private void Start()
    {
        _getAnyKey = GetComponentInChildren<Text>();
        _textAnimation = _getAnyKey.DOFade(1, textTimer);
        _textAnimation.SetLoops(-1, LoopType.Yoyo);
        ClientHub.Instance.Client.onEndTCPConnect += client =>
        {
            OnAnyKeyDone();
        };
        ClientHub.Instance.Client.onConnectionFailed += (client, e) =>
        {
            _stop = false;
        };
        GetComponentInChildren<Button>().onClick.AddListener(QuitGame);
    }
    private void QuitGame()
    {
        Application.Quit();
    }
    public override void OnClose()
    {
        UIManager.DefaultOC(canvasGroup, false, 1f);
    }
    public void OnAnyKeyDone()
    {
        _textAnimation.Pause();
        _getAnyKey.DOFade(0, 0.5f).onComplete += () =>
        {
            uiManager.CloseUI(this);
            uiManager.OpenUI(UIType.LoginPanel);
        };
    }

    public override void OnOpen()
    {
        _stop = false;
        _textAnimation?.Play();
        UIManager.DefaultOC(canvasGroup, true, 1f);
    }
    private void GetAnyKeyEvent()
    {
        if (!ClientHub.Instance.RetryServer())
            return;
        OnAnyKeyDone();
    }

    public void UpdateView()
    {
        if (Input.anyKeyDown && !_stop)
        {
            _stop = true;
            GetAnyKeyEvent();
        }
    }

}
