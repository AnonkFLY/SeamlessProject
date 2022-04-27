using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UI;
using UI.BaseClass;
using UI.BaseClass.Interface;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InputView : UIController, IRequirementData, IUpdatable
{
    private TMP_InputField _inputField;
    private TMP_Text _placeholder;
    private float _orginY;
    private Action onClose;
    private Action onOpen;
    private event Action<InputView, string> onEndEditor;


    public override void RegisterUIManager(UIManager uiManager)
    {
        base.RegisterUIManager(uiManager);
        _inputField = GetComponentInChildren<TMP_InputField>();
        _placeholder = GetComponentInChildren<TMP_Text>();

        _inputField.onEndEdit.AddListener(InputHandler);
        _orginY = transform.localPosition.y;



        var tween = transform.DOLocalMoveY(_orginY - rectTrans.rect.height, 1);
        tween.Pause();
        tween.SetAutoKill(false);
        onClose += tween.PlayForward;
        onOpen += tween.PlayBackwards;
    }
    public override void OnClose()
    {
        onClose?.Invoke();
        _inputField.interactable = false;
    }

    public override void OnOpen()
    {
        onOpen?.Invoke();
        _inputField.interactable = true;
    }
    private void ResetInputField()
    {
        _inputField.text = "";
    }
    public bool InComingData(object data)
    {
        try
        {
            var eventData = (InputEventData)data;
            onEndEditor = eventData.onEnter;
            _placeholder.text = eventData.inputText;
            _inputField.contentType = eventData.type;
            return true;
        }
        catch
        {
            return false;
        }
    }
    private void InputHandler(string inputText)
    {
        onEndEditor?.Invoke(this, inputText);
    }
    public void ResetInput()
    {
        _inputField.text = "";
        uiManager.CloseUI(type);
    }

    public void UpdateView()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uiManager.CloseUI(type);
        }
    }
}
