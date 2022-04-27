using System;
using UI.BaseClass;
using UnityEditor;
using UnityEngine;

public abstract class RoomController
{
    protected RoomData roomType;
    private PlayerList _playerList;
    protected GameObject _sceneObj;
    protected ResourceRequest obj;
    public Action<GameObject, RoomController> onLoaded;
    public Action<GameObject> onLoadedUIOver;

    public PlayerList PlayerList { get => _playerList; }

    public void InitRoomData(RoomData data, GameObject playerPrefab, GameObject self)
    {
        _playerList = new PlayerList(playerPrefab, self);
        LoadScene(data.type);
    }

    private void LoadScene(RoomType type)
    {
        GameManager.Instance.uiManager.OpenUI(UI.BaseClass.UIType.LoadingPanel);
        //UnityEngine.Debug.Log(type);
        switch (type)
        {
            case RoomType.TestRoom:
                AsyncLoadScene("ScenePrefabs/TestScene");
                break;
            case RoomType.Game1:
                AsyncLoadScene("ScenePrefabs/GameScene");
                break;
            default:
                AsyncLoadScene("ScenePrefabs/TestScene");
                break;
        }
    }
    private void AsyncLoadScene(string path)
    {
        GameManager.Instance.assetManager.LoadAsyncAsset<GameObject>(path, obj =>
        {
            this.obj = obj;
            var prefab = (GameObject)obj.asset;
            _sceneObj = GameObject.Instantiate(prefab);
            var uiManager = GameManager.Instance.uiManager;
            PacketSendContr.SendClientJoinRoom();
            uiManager.CloseAllUI();
            uiManager.OpenUI(UIType.GameInfoPanel, _playerList);
            uiManager.SetBackOpen(false, 3f).onComplete += () => { onLoadedUIOver?.Invoke(_sceneObj); };
            onLoaded?.Invoke(prefab, this);
        });
    }
    public void CloseController()
    {
        _playerList.CloseList();
        _playerList = null;
        GameObject.Destroy(_sceneObj);
        Resources.UnloadUnusedAssets();
        UnityEngine.Debug.Log(obj);
    }

}
