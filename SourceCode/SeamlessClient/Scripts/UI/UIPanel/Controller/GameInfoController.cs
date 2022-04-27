using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnonSocket.Data;
using DG.Tweening;
using UI;
using UI.BaseClass;
using UI.BaseClass.Interface;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoController : UIController, IUpdatable, IRequirementData, ICoverable
{
    private readonly int openHash = Animator.StringToHash("Open");
    private readonly int closeHash = Animator.StringToHash("Close");

    private PlayerInfoView _playerInfo;
    private ScoreboardData _scoreboardView;
    private ChatBoxView _chatBoxView;
    private Animator _animator;
    private Button _operationButton;
    private Text _infoText;
    [SerializeField] private Text infoTitle;
    [SerializeField] private Text infoSubTitle;
    [SerializeField] private GameObject _startGameButton;
    private WeaponManager _weaponManager;
    private Vector3 _subTitleOrigin;
    public static Action gameInfoPanelUpdate;
    private SelfPlayer player;
    public override void RegisterUIManager(UIManager uiManager)
    {
        base.RegisterUIManager(uiManager);
        PlayerList.onInitSelf += player => this.player = player;
        _subTitleOrigin = infoSubTitle.transform.position;
        _infoText = transform.Find("InfoText").GetComponent<Text>();
        ClientHub.Instance.onPingDone += Ping;
        _scoreboardView = GetComponentInChildren<ScoreboardData>();
        _playerInfo = GetComponentInChildren<PlayerInfoView>();
        _chatBoxView = GetComponentInChildren<ChatBoxView>();
        _scoreboardView.SetInfoController(this);
        _operationButton = transform.Find("OperationButton").GetComponent<Button>();
        _weaponManager = new WeaponManager(transform);
        GameManager.Instance.weaponManager = _weaponManager;
        _operationButton.onClick.AddListener(() => { uiManager.OpenUI(UIType.OperationPanel); });
        _animator = GetComponent<Animator>();
        PacketHandlerRegister.RegisterPlayerScoreChange(ScoreChangeHandler);
        GameManager.Instance.onOverGame += OpenStartButton;

    }
    private void Ping(int time)
    {
        _infoText.text = "Ping: " + time;
    }

    private void ScoreChangeHandler(PacketBase packet)
    {
        _scoreboardView.UpdateDataOnPlayer(packet.ReadULong(), packet.ReadByte(), packet.ReadInt32());
    }

    public override void OnOpen()
    {
        _animator.Play(openHash);
        OpenStartButton();
    }
    private void OpenStartButton()
    {
        if (GameManager.Instance.isHomowenr)
            _startGameButton.SetActive(true);
    }
    public void SetGoldCount(int count)
    {
        _playerInfo.GoldCount = count;
    }
    private void OnJoinPlayer(PlayerData data)
    {
        _scoreboardView.AddPlayer(data.uid, data.GetName());
    }
    private void OnQuitPlayer(PlayerData data)
    {
        _scoreboardView.RemovePlayer(data.uid);
    }

    public override void OnClose()
    {
        _animator.Play(closeHash);
        _chatBoxView.ResetText();
        _playerInfo.Close();
        _startGameButton.SetActive(false);
    }

    public void UpdateView()
    {
        _chatBoxView?.NeedUpdate();
        if (Input.GetKeyDown(KeyCode.Tab))
            _scoreboardView.Open();
        if (Input.GetKeyUp(KeyCode.Tab))
            _scoreboardView.Close();
        if (player && !player.isInput)
            return;
        gameInfoPanelUpdate?.Invoke();
        if (Input.GetKeyDown(KeyCode.Q))
            _weaponManager.ThrowWeapon();
        if (Input.GetKeyDown(KeyCode.Alpha1))
            _weaponManager.OnSelectIndex(3);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            _weaponManager.OnSelectIndex(2);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            _weaponManager.OnSelectIndex(1);
    }

    public bool InComingData(object data)
    {
        PlayerList list = (PlayerList)data;
        list.onJoinPlayer += OnJoinPlayer;
        list.onQuitPlayer += OnQuitPlayer;
        return true;
    }

    public void OnPause()
    {
        GameManager.Instance.selfPlayerObj.isInput = false;
    }

    public void OnRenew()
    {
        GameManager.Instance.selfPlayerObj.isInput = true;
    }
    public void ShowTitle(string text)
    {
        infoTitle.text = text;
    }
    public void ShowSubTitle(string text, float timer)
    {
        ShotTitleText(infoSubTitle, text, timer);
    }
    private void ShotTitleText(Text textText, string text, float timer)
    {
        textText.text = text;
        textText.DOFade(1, 0);
        textText.DOFade(1, timer).onComplete += () =>
        {
            var trans = textText.transform;
            trans.position = _subTitleOrigin;
            trans.DOMoveY(trans.position.y - 30, 2);
            textText.DOFade(0, 2);
        };
    }
}
