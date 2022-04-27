using System.Collections;
using System.Collections.Generic;
using UI;
using UI.BaseClass;
using UnityEngine;
using UnityEngine.UI;

public class UserDataView : MonoBehaviour
{
    [SerializeField] private Transform texts;
    [SerializeField] private Button changeGameNameButton;
    [SerializeField] private Button changePasswordButton;
    [SerializeField] private Button signOutButton;
    private ViewPlayerInfo _playerInfo;
    private UIManager _uiManager;
    private InputEventData _onChangeGameNameData;
    private InputEventData _onChangePasswordData;
    private void Awake()
    {
        InitValue();
        InitPanelEvent();
    }
    private void InitValue()
    {
        _playerInfo = new ViewPlayerInfo(texts);
        _uiManager = GameManager.Instance.uiManager;
        _onChangeGameNameData = new InputEventData("输入游戏名称...", ChangeGameNameHandler, TMPro.TMP_InputField.ContentType.Standard);
        _onChangePasswordData = new InputEventData("输入新密码...", ChangePasswordHandler, TMPro.TMP_InputField.ContentType.Password);
    }
    private void InitPanelEvent()
    {
        signOutButton.onClick.AddListener(SignOutLogin);
        changeGameNameButton.onClick.AddListener(ChangeGameName);
        changePasswordButton.onClick.AddListener(ChangePassword);

        ClientHub.Instance.onUpdatePlayerData += data =>
        {
            _playerInfo.UpdatePlayerData(data);
        };
    }
    private void SignOutLogin()
    {
        ClientHub.Instance.SignOut();
    }
    private void ChangeGameName()
    {
        _uiManager.OpenUI(UIType.InputViewPanel, _onChangeGameNameData);
    }
    private void ChangePassword()
    {
        _uiManager.OpenUI(UIType.InputViewPanel, _onChangePasswordData);
    }
    private void ChangeGameNameHandler(InputView inputView, string input)
    {
        if (input.Length < 2 || input.Length > 10)
        {
            _uiManager.OpenUI(UIType.PromptPanel, "GameName should be between 6 and 20 digits\n游戏名应在2到10位");
            return;
        }
        PacketSendContr.SendGameName(input);
        ChangeDone(inputView);
    }
    private void ChangePasswordHandler(InputView inputView, string input)
    {
        if (input.Length < 6 || input.Length > 20)
        {
            _uiManager.OpenUI(UIType.PromptPanel, "Password should be between 6 and 20 digits\n密码应在6到20位");
            return;
        }
        PacketSendContr.SendChangePassword(input);
        ChangeDone(inputView);
    }
    private void ChangeDone(InputView inputView)
    {
        inputView.ResetInput();
        _uiManager.OpenUI(UIType.PromptPanel, "修改成功!");
        _uiManager.CloseUI(inputView.type);
    }
}
