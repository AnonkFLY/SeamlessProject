using System.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using AnonSocket.Data;
using IO;
using UI;
using UI.BaseClass;
using UI.BaseClass.Interface;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private List<Action> mainThreadAction = new List<Action>();
    //[SerializeField] private List<UIController> uis;

    [SerializeField] private GameObject selfPlayer;
    [SerializeField] private GameObject otherPlayer;

    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }
    private Vector3 _cameraOrigin;
    public UIManager uiManager;
    public AssetManager assetManager;
    public UIType awakeOpenPanel;
    public UIType[] awakeLoadPanel;
    public UnityAction updateAction;
    public BulletManager bulletManager;
    public WeaponManager weaponManager;
    public SelfPlayer selfPlayerObj;

    public RoomController roomController;
    public Action<RoomController> onJoinRoom;
    public Action onQuitRoom;
    public bool isHomowenr = false;
    [SerializeField] Text text;


    private void Awake()
    {
        _cameraOrigin = Camera.main.transform.position;
        selfPlayerObj = FindObjectOfType<SelfPlayer>();
        InitSingleton();
        assetManager = FindObjectOfType<AssetManager>();
        uiManager = new UIManager(assetManager);
        bulletManager = new BulletManager();
        uiManager.onTopChange += GetUIPanelChangeEvent;
    }


    public void SignOut()
    {
        uiManager.CloseUI(UIType.MainPagePanel);
        DelayExecute(1.5f, () => { uiManager.OpenUI(UIType.StartGamePanel); });
    }

    private void Start()
    {
        uiManager.OpenUI(awakeOpenPanel);
        StartCoroutine(LoadUICoroutine(awakeLoadPanel));
        OnInitEvent();
    }

    private IEnumerator LoadUICoroutine(UIType[] loadPanels)
    {
        var wait = new WaitForSeconds(0.2f);
        foreach (var item in loadPanels)
        {
            uiManager.InstancePanel(item);
            yield return wait;
        }
    }

    private void Update()
    {
        updateAction?.Invoke();
        lock (mainThreadAction)
        {
            try
            {

                if (mainThreadAction.Count > 0)
                {

                    for (int i = 0; i < mainThreadAction.Count; i++)
                    {
                        try
                        {
                            mainThreadAction[i]?.Invoke();
                        }
                        catch (Exception e)
                        {
                            print($"Error:" + e);
                            continue;
                        }
                    }
                    mainThreadAction.Clear();
                }
            }
            catch (Exception e)
            {
                uiManager.OpenUI(UIType.ErrorInfoPanel, new ErrorPrompt("Yes", e.ToString()));
                mainThreadAction.Clear();
            }
        }


        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     uiManager.OpenUI(UIType.LoadingPanel);
        // }
        // if (Input.GetKeyDown(KeyCode.L))
        // {
        //     OnQuitRoom();
        // }
        // if (Input.GetKeyDown(KeyCode.F))
        // {
        //     //bulletManager.CreateBullet(BulletType.Pistol, Vector3.up * 5, Vector3.right * 10);
        //     //uiManager.OpenUI(UIType.InputViewPanel);
        // }
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     uiManager.CloseUI();//UnityEditor.EditorApplication.isPlaying = false;
        // }
    }
    private void InitSingleton()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(this.gameObject);
    }
    private void GetUIPanelChangeEvent(UIController oldPanel, UIController newPanel, bool open)
    {
        var updateOld = oldPanel?.GetComponent<IUpdatable>();
        if (updateOld != null)
        {
            updateAction -= updateOld.UpdateView;
        }
        var updateNew = newPanel?.GetComponent<IUpdatable>();
        if (updateNew != null)
        {
            updateAction += updateNew.UpdateView;
        }
        if (open)
        {
            var stationedUpdate = newPanel?.GetComponent<IStationedUpdate>();
            if (stationedUpdate != null)
                updateAction += stationedUpdate.StationedUpdate;
        }
        else
        {
            var stationedUpdate = oldPanel?.GetComponent<IStationedUpdate>();
            if (stationedUpdate != null)
                updateAction -= stationedUpdate.StationedUpdate;
        }
    }
    public void OnMainThreadExecute(Action action)
    {
        mainThreadAction.Add(action);
    }
    public void DelayExecute(float delayTimer, Action action)
    {
        StartCoroutine(DelayExecuteCoroutine(delayTimer, action));
    }
    private IEnumerator DelayExecuteCoroutine(float delayTimer, Action action)
    {
        yield return new WaitForSeconds(delayTimer);
        action?.Invoke();
    }
    private void OnInitEvent()
    {
        ClientHub.Instance.onJoinRoom += SwitchJoinRoom;
    }
    private void SwitchJoinRoom(int result, RoomData roomData)
    {
        switch (result)
        {
            case 0:
                uiManager.OpenUI(UIType.PromptPanel, "人数已满");
                break;
            case 1:
                uiManager.OpenUI(UIType.PromptPanel, "加入失败!");
                break;
            default:
                uiManager.OpenUI(UIType.PromptPanel, "正在进入房间...");
                PacketHandlerRegister.RegisterCreateItemHanlder();
                PacketHandlerRegister.RegisterCreateBulletHanlder(CreateBulletHandler);
                uiManager.CloseUI(UIType.MainPagePanel);
                OnJoinBack(roomData);
                break;
        }
    }

    private void CreateBulletHandler(PacketBase packet)
    {
        bulletManager.CreateBullet((BulletType)packet.ReadByte(), packet.ReadVector3(), packet.ReadVector3());
    }

    private void OnJoinBack(RoomData roomData)
    {
        roomController = new TestRoom();
        onJoinRoom?.Invoke(roomController);
        roomController.InitRoomData(roomData, otherPlayer, selfPlayer);

        text.text = "等待房主开始游戏...";
    }
    public void Message(string message)
    {
        uiManager.OpenUI(UIType.PromptPanel, message);
    }
    public void OnQuitRoom()
    {
        text.text = "";
        if (roomController == null)
            return;
        PacketSendContr.SendQuitRoom();
        uiManager.CloseUI(UIType.GameInfoPanel);
        uiManager.CloseUI(UIType.OperationPanel);
        uiManager.OpenUI(UIType.PromptPanel, "正在退出房间...");
        uiManager.SetBackOpen(true, 1);
        uiManager.OpenUI(UIType.MainPagePanel);
        roomController.CloseController();
        roomController = null;
        assetManager.ItemManager.DestroyAllItem();
        isHomowenr = false;
        onQuitRoom.Invoke();
        Camera.main.transform.position = _cameraOrigin;
    }
    public event Action onStartGame;
    public event Action onOverGame;
    public void OnStartGame(int time)
    {
        if (roomController == null)
            return;
        if (time <= 0)
        {
            text.text = $"开始倒计时\n<size=30>{(-time - 1).ToString()}</size>";
        }
        if (time == -1)
        {
            text.text = "<size=20>开始游戏</size>";
            weaponManager.CloseAll();
            onStartGame?.Invoke();
        }
        if (time > 0)
        {
            text.text = $"<size=25><color=#8FEDFF>剩余时间</color></size>\n<size=12><b>{(time / 60)}:{(time % 60)}</b></size>";
        }
        if (time == 0)
            StartCoroutine(GameOver());
    }
    private IEnumerator GameOver()
    {
        text.text = $"游戏结束";
        onOverGame?.Invoke();
        yield return new WaitForSeconds(1);
        if (!isHomowenr)
            text.text = "等待房主开始游戏...";
    }
    public void GetItemHandler(byte data, int ammon, int current)
    {
        GetWeapon(data, ammon, current);
    }
    private void GetWeapon(byte data, int ammon, int current)
    {
        var weapon = (WeaponType)data;
        var obj = assetManager.GetWeapon(weapon);
        weaponManager.GetWeapon(obj, ammon, current);
    }
}
