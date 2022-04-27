using System;
using UnityEngine;
using UnityEngine.UI;

public class KeyIconView
{
    private Button _keyButton;
    private Text _countDownText;
    private Image _countDownMask;
    private Image _itemImage;
    private KeyCode _keyEvent;
    private float _currentCooling;
    private float _maxCooling;

    private IUserable item;
    public event Action<float> onCountDown;
    public event Action<float> onCoolingDone;

    public KeyIconView(Transform transform, KeyCode keyEvent)
    {
        _keyButton = transform.GetComponentInChildren<Button>();
        _itemImage = transform.Find("ItemImage").GetComponent<Image>();
        _countDownMask = transform.Find("ItemImageMask").GetComponent<Image>();
        _countDownText = _countDownMask.GetComponentInChildren<Text>();
        _keyEvent = keyEvent;
    }
    public void IsKeyDown(float timer)
    {
        if (_currentCooling > 0)
        {
            CoolingCountDown(timer);
            return;
        }
        if (Input.GetKeyDown(_keyEvent))
        {
            IntoCooling();
            if (PlayerList.selfPlayer)
                item?.UseItem(PlayerList.selfPlayer);
        }
    }
    private void CoolingCountDown(float removeValue)
    {
        _currentCooling -= removeValue;
        _countDownMask.fillAmount = _currentCooling / _maxCooling;
        _countDownText.text = _maxCooling.ToString("f1");
        if (_currentCooling <= 0)
        {
            _currentCooling = 0;
            _keyButton.interactable = true;
            _countDownText.text = "";
            //cooling over
        }
    }
    private void IntoCooling()
    {
        _currentCooling = _maxCooling;
        _countDownMask.fillAmount = 1;
        _keyButton.interactable = false;
        _countDownText.text = _maxCooling.ToString("f1");
    }
    public void SetItem(IUserable item)
    {
        this.item = item;
        _maxCooling = item.GetCooling();
    }
}
