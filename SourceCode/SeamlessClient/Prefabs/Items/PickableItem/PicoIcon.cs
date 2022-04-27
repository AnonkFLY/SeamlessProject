using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PicoIcon : MonoBehaviour
{
    private CanvasGroup _canvaGroup;
    private Transform _childImage;
    [SerializeField] private int count;
    GameInfoController gameInfo;
    private void Awake()
    {
        _canvaGroup = GetComponent<CanvasGroup>();
        GameInfoController.gameInfoPanelUpdate += NeedUpdate;
    }
    private void NeedUpdate()
    {
        var player = GameManager.Instance.selfPlayerObj;
        if (player != null && player.State == 1)
            return;
        if (count == 0)
            return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (GameManager.Instance.weaponManager.IsMaxWeapon())
            {
                var gameInfo = GameManager.Instance.uiManager.GetUIController<GameInfoController>(UI.BaseClass.UIType.GameInfoPanel);
                gameInfo?.ShowSubTitle("武器已满", 1);
                return;
            }
            PacketSendContr.PickupAsk();
            count--;
        }
    }
    public void Open()
    {
        count++;
        _canvaGroup.DOFade(1, 0.8f);
        return;
    }

    public void Close()
    {
        RemoveCount();
    }
    public void RemoveCount()
    {
        count--;
        if (count == 0)
            _canvaGroup.DOFade(0, 0.8f);
    }
}
