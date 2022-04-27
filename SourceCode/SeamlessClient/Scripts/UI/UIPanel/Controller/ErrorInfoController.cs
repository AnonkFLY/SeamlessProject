using System;
using DG.Tweening;
using UI;
using UI.BaseClass;
using UI.BaseClass.Interface;
using UnityEngine;
using UnityEngine.UI;

public class ErrorInfoController : UIController, IRequirementData
{
    [Range(0, 2)]
    [SerializeField]
    private float _fadeTimer = 1;
    [Range(0, 2)]
    [SerializeField]
    private float _scaleTimer = 0.5f;
    private Text _errorText;
    private Text _retryText;
    private Button _retryButton;
    private Button _closeButton;
    private Transform _windowTrans;
    private CanvasGroup _windowCanvesGroup;

    public bool InComingData(object data)
    {
        try
        {
            var errorData = (ErrorPrompt)data;
            _retryText.text = errorData.confirmText;
            _errorText.text = errorData.infoText;
            if (errorData.confirmButtonAction != null)
                _retryButton.onClick.AddListener(errorData.confirmButtonAction);
            else
                _retryButton.onClick.AddListener(() =>
                {
                    uiManager.CloseUI(type);
                });
            if (errorData.closeButtonAction != null)
                _closeButton.onClick.AddListener(errorData.closeButtonAction);
            else
                _closeButton.onClick.AddListener(() =>
                {
                    uiManager.CloseUI(type);
                });
            return true;
        }
        catch (Exception e)
        {
            _retryText.text = "Yes";
            _errorText.text = e.ToString();
            return false;
        }
    }

    public override void OnClose()
    {
        _closeButton.onClick.RemoveAllListeners();
        _retryButton.onClick.RemoveAllListeners();
        uiManager.onTopChange -= StationedTopPanel;
        UIManager.DefaultOC(canvasGroup, false, 0.3f);
        UIManager.DefaultOC(_windowCanvesGroup, false, _fadeTimer);
        _windowTrans.DOScale(0, _scaleTimer);
    }

    public override void OnOpen()
    {
        uiManager.onTopChange -= StationedTopPanel;
        uiManager.onTopChange += StationedTopPanel;
        UIManager.DefaultOC(canvasGroup, true, 0.3f);
        UIManager.DefaultOC(_windowCanvesGroup, true, _fadeTimer);
        _windowTrans.DOScale(1, _scaleTimer);
    }

    private void StationedTopPanel(UIController oldPanel, UIController newPanel, bool open)
    {
        if (!open)
            return;
        if (oldPanel != null && (oldPanel == this && newPanel != this))
        {
            //print($"debug:{oldPanel.transform.name} and {newPanel?.transform.name}");

            uiManager.OpenUI(type);
        }
    }

    public override void RegisterUIManager(UIManager uiManager)
    {
        base.RegisterUIManager(uiManager);
        var texts = GetComponentsInChildren<Text>();
        _windowTrans = transform.GetChild(0);
        _windowTrans.localScale = Vector3.zero;
        _windowCanvesGroup = _windowTrans.GetComponent<CanvasGroup>();
        _errorText = texts[0];
        _retryText = texts[1];
        var buttons = GetComponentsInChildren<Button>();
        _closeButton = buttons[0];
        _retryButton = buttons[1];

    }
}
