using System.Diagnostics;
using System;
using TMPro;
using UI.BaseClass;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;
using AnonSocket.Data;

namespace UI.Panels
{
    public class LoginUIView : UIView
    {
        private RectTransform _rectTransform;
        private TMP_InputField _usernameInputField;
        private TMP_InputField _passwordInputField;
        private TMP_InputField _passwordAgainInputField;
        private CanvasGroup _passwordAgainCanvesGroup;
        private Button _registerButton;
        private Button _loginButton;
        private bool _isRegisterPage;
        private bool _isChanging;

        private Vector2 _loginRect;
        public Action<ulong> onTryLogin;
        private Animator _animator;

        public override void InitView(Transform transform)
        {
            _animator = transform.GetComponent<Animator>();
            _rectTransform = transform.GetComponent<RectTransform>();
            _loginRect = _rectTransform.sizeDelta;
            var inputFields = transform.GetComponentsInChildren<TMP_InputField>();
            var buttons = transform.GetComponentsInChildren<Button>();
            _usernameInputField = inputFields[0];
            _passwordInputField = inputFields[1];
            _passwordAgainInputField = inputFields[2];
            _passwordAgainCanvesGroup = _passwordAgainInputField.GetComponent<CanvasGroup>();
            _loginButton = buttons[0];
            _registerButton = buttons[1];
            InitEvent();
        }

        private void InitEvent()
        {
            _registerButton.onClick.AddListener(ToRegisterPage);
            _loginButton.onClick.AddListener(Login);
            PacketHandlerRegister.RegisterUserHandler(packet =>
            {
                var uid = packet.ReadULong();
                if (uid == 0)
                {
                    GameManager.Instance.uiManager.OpenUI(UIType.PromptPanel, "Duplicate username\n重复的用户名");
                    return;
                }
                GameManager.Instance.uiManager.OpenUI(UIType.PromptPanel, $"The registration is successful\n注册成功");
                ToLoginPage();
                ResetPassword();
            });
            PacketHandlerRegister.RegisterLoginHandler(packet =>
            {
                var result = packet.ReadULong();
                //UnityEngine.Debug.Log("登录结果:" + result);
                LoginHandler(result);
                ResetPassword();
                onTryLogin?.Invoke(result);
                if (result > 10)
                {
                    ClientHub.Instance.ChangeState(ClientState.Loging);
                }
            });
        }

        public void ResetPassword()
        {
            _passwordInputField.text = "";
            _passwordAgainInputField.text = "";
        }
        public LoginData GetLoginData()
        {
            return new LoginData(_usernameInputField.text, _passwordInputField.text, _passwordAgainInputField.text);
        }
        public void ToRegisterPage()
        {
            if (_isRegisterPage || _isChanging)
                return;
            GameManager.Instance.OnMainThreadExecute(() => { ChangePageAnimation(true); });
        }


        public void ToLoginPage()
        {
            if (!_isRegisterPage || _isChanging)
                return;
            GameManager.Instance.OnMainThreadExecute(() => { ChangePageAnimation(false); });
        }
        private void ChangePageAnimation(bool isRegister)
        {
            var timer = 0.5f;
            var alpha = isRegister ? 1 : 0;
            var fAlpha = isRegister ? 0 : 1;
            var inputFiledTrans = _passwordAgainCanvesGroup.GetComponent<RectTransform>();
            //var hight = ScreenSizeUtil.GetSize(110);
            //UnityEngine.Debug.Log(hight);
            //hight = (isRegister ? -hight : 0);
            if (isRegister)
                SetAgainInteractable(isRegister);
            _passwordAgainCanvesGroup.DOFade(alpha, timer);
            _animator.Play(isRegister ? "Open" : "Close");
            Text[] loginButtonTexts = _loginButton.GetComponentsInChildren<Text>();
            Text[] changePage = _registerButton.GetComponentsInChildren<Text>();
            loginButtonTexts[0].DOFade(fAlpha, timer).onComplete += () => { OnPanelChangeStateOnCopmleteEvent(isRegister); };
            loginButtonTexts[1].DOFade(alpha, timer);
            changePage[0].DOFade(alpha, timer);
            changePage[1].DOFade(fAlpha, timer);

            //var endValue = _loginRect + (isRegister ? Vector2.up * -hight : Vector2.zero);
            //_rectTransform.DOSizeDelta(endValue, timer);
        }
        private void SetAgainInteractable(bool value)
        {
            _passwordAgainCanvesGroup.interactable = value;
            _passwordAgainCanvesGroup.blocksRaycasts = value;
        }
        private void OnPanelChangeStateOnCopmleteEvent(bool isRegister)
        {
            ResetPassword();
            if (!isRegister)
                SetAgainInteractable(isRegister);
            _isRegisterPage = isRegister;
            _isChanging = false;
            if (isRegister)
            {
                _registerButton.onClick.RemoveListener(ToRegisterPage);
                _registerButton.onClick.AddListener(ToLoginPage);
                OnSetRegisterEvent();
            }
            else
            {
                _registerButton.onClick.RemoveListener(ToLoginPage);
                _registerButton.onClick.AddListener(ToRegisterPage);
                OnSetLoginEvent();
            }
        }
        private void OnSetLoginEvent()
        {
            _loginButton.onClick.RemoveListener(Register);
            _loginButton.onClick.AddListener(Login);
        }
        private void OnSetRegisterEvent()
        {
            _loginButton.onClick.RemoveListener(Login);
            _loginButton.onClick.AddListener(Register);
        }
        private void Register()
        {
            var data = GetLoginData();
            if (data.passWord != data.passWordAgain)
            {
                GameManager.Instance.uiManager.OpenUI(UIType.PromptPanel, "Different passwords twice\n两次输入的密码不同");
                return;
            }
            if (data.passWord.Length < 6 || data.passWord.Length > 20)
            {
                GameManager.Instance.uiManager.OpenUI(UIType.PromptPanel, "Password should be between 6 and 20 digits\n密码应在6到20位");
                return;
            }
            if (data.userName.Length < 6 || data.userName.Length > 20)
            {
                GameManager.Instance.uiManager.OpenUI(UIType.PromptPanel, "Username should be between 6 and 20 digits\n用户名应在6到20位");
                return;
            }
            PacketSendContr.SendRegisterData(data);
        }
        private void Login()
        {
            var data = GetLoginData();
            UnityEngine.Debug.Log("Try Login");
            PacketSendContr.SendLoginData(data);
        }
        private void LoginHandler(ulong uid)
        {
            /// 3 Online user
            /// 2 Wrong password
            /// 1 Username does not exist
            /// 0 other wrong
            string debug;
            switch (uid)
            {
                case 0:
                    debug = $"Other wrong\n其他错误";
                    break;
                case 1:
                    debug = $"Username does not exist\n用户名不存在";
                    break;
                case 2:
                    debug = $"Wrong passWord\n错误的密码";
                    break;
                case 3:
                    debug = $"The user is online\n该用户已在线上";
                    break;
                default:
                    debug = $"Successful landing\n登陆成功";
                    break;
            }
            GameManager.Instance.uiManager.OpenUI(UIType.PromptPanel, debug);
        }
    }
}