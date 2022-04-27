using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UI;
using UI.BaseClass;
using UI.BaseClass.Interface;
using UnityEngine;
using UnityEngine.UI;

public class PromptPanel : UIController, IRequirementData
{
    private Text _promptText;
    private Transform _textTrans;
    private Vector3 _orginPosition;
    private Color _orginColor;
    private Queue<string> _textQueue;
    private bool _isShowing = false;
    [SerializeField] private float _distance = 30f;
    [SerializeField] private float _timer = 2;
    public override void RegisterUIManager(UIManager uiManager)
    {
        base.RegisterUIManager(uiManager);
        _promptText = GetComponentInChildren<Text>();
        _textTrans = _promptText.transform;
        _orginPosition = _textTrans.position;
        _orginColor = _promptText.color;
        _textQueue = new Queue<string>();
    }
    public bool InComingData(object data)
    {
        try
        {
            _textQueue.Enqueue(data.ToString());
            return true;
        }
        catch (Exception e)
        {
            print(e);
            return false;
        }
    }

    public override void OnClose()
    {

    }

    public override void OnOpen()
    {
        if (!_isShowing)
        {
            _isShowing = true;
            OverALine();
        }
    }
    private void ShowText()
    {
        _promptText.text = _textQueue.Dequeue();
        _promptText.color = _orginColor;
        _textTrans.position = _orginPosition;
        _textTrans.DOMoveY(_orginPosition.y + _distance, _timer).onComplete += () =>
        {
            _promptText.DOFade(0, 0.5f).onComplete += OverALine;
        };
    }
    private void OverALine()
    {
        if (_textQueue.Count <= 0)
        {
            _isShowing = false;
            uiManager.CloseUI(this);
        }
        else
        {
            ShowText();
        }
    }
}
