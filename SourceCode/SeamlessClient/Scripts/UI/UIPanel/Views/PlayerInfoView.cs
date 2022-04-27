using System;
using System.Collections;
using System.Collections.Generic;
using AnonSocket.AnonServer;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class PlayerInfoView : MonoBehaviour
{
    [SerializeField] private Transform sliderParent;
    [SerializeField] private Transform KeyDownIconParent;
    private SliderView _healthSlider;
    private SliderView _armorSlider;
    [SerializeField] private Image _vigourImage;
    private TMP_Text _userNameText;
    private TMP_Text _goldCountText;

    private KeyIconView _skillButton;
    private KeyIconView _propButton;

    private int _goldCount;

    public int GoldCount
    {
        get => _goldCount;
        set
        {
            _goldCount = value;
            _goldCountText.text = "钱币数量: " + _goldCount.ToString();
        }
    }

    private void Awake()
    {
        _healthSlider = new SliderView(sliderParent.GetChild(0));
        _armorSlider = new SliderView(sliderParent.GetChild(1));
        _userNameText = sliderParent.GetChild(2).GetComponent<TMP_Text>();
        _goldCountText = sliderParent.GetChild(3).GetComponent<TMP_Text>();
        _armorSlider.isSegmented = true;
        var name = ClientHub.Instance.GetPlayerName();
        _userNameText.text = "用户名: " + name;

        _skillButton = new KeyIconView(KeyDownIconParent.GetChild(0), KeyCode.Q);
        _propButton = new KeyIconView(KeyDownIconParent.GetChild(0), KeyCode.E);
        PlayerList.onInitSelf += PlayerCreate;
    }

    private void PlayerCreate(SelfPlayer player)
    {
        var info = player.InfoData;
        info.armor.onValueChange += _armorSlider.SetValue;
        info.health.onValueChange += _healthSlider.SetValue;
        info.vigour.onValueChange += (cur, max) =>
        {
            //            print($"精力:{cur},最大{max}");
            _vigourImage.fillAmount = cur / max;
        };
    }
    public void Close()
    {
        _healthSlider.Reset();
        _armorSlider.Reset();
    }
}
