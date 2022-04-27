using System.Collections;
using System.Collections.Generic;
using AnonSocket.Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ChatBoxView : MonoBehaviour
{
    [SerializeField] private InputField inputField;
    [SerializeField] private Image textBack;
    [SerializeField] private Text textCom;
    private CanvasGroup _inputFieldCanvas;
    private float backAlpha;
    private float canvesAlpha;
    private void Awake()
    {
        _inputFieldCanvas = inputField.GetComponent<CanvasGroup>();
        backAlpha = textBack.color.a;
        canvesAlpha = _inputFieldCanvas.alpha;
        textBack.DOFade(0, 0);
        _inputFieldCanvas.alpha = 0;
        PacketHandlerRegister.RegisterChatBox(ReceiveMessageHandler);
        inputField.onEndEdit.AddListener(InputHanlder);
    }
    public void NeedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnEnd();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnOut();
        }
    }
    private void ReceiveMessageHandler(PacketBase packet)
    {
        var name = packet.ReadString();
        var message = packet.ReadString();
        if (name == "AnonkServer")
        {
            textCom.text += $"{message}\n";
            return;
        }
        textCom.text += $"[{name}]{message}\n";
    }
    public void ResetText()
    {
        textCom.text = "";
    }
    private void InputHanlder(string input)
    {
        if (string.IsNullOrEmpty(input))
            return;
        PacketSendContr.SendMessageString(input);
        inputField.text = "";
    }
    private void OnEnd()
    {
        GameManager.Instance.selfPlayerObj.isInput = false;
        inputField.interactable = true;
        inputField.ActivateInputField();
        textBack.DOFade(backAlpha, 0.4f);
        _inputFieldCanvas.DOFade(canvesAlpha, 0.4f);
    }
    private void OnOut()
    {
        textBack.DOFade(0, 0.4f);
        inputField.interactable = false;
        _inputFieldCanvas.DOFade(0, 0.4f);
        GameManager.Instance.selfPlayerObj.isInput = true;
    }
}
