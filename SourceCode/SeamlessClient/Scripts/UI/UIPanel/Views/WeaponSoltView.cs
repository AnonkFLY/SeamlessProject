using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WeaponSoltView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region UnityUI
    private Image _weaponIcon;
    private CanvasGroup _selectRect;
    private TMP_Text _ammunitionText;
    private TMP_Text _weaponNameText;
    private TMP_Text _weaponDescride;
    private CanvasGroup _weaponDescrideBack;
    private Image _loadImage;
    private TMP_Text _loadText;
    private Button _soltButton;
    #endregion
    [SerializeField] private FirearmsItem _firearm;
    private WeaponManager _weaponManager;
    private bool isSelect = false;
    private int _soltID;
    private Sprite alpahSprite;
    public FirearmsItem Firearm { get => _firearm; }
    private void Awake()
    {
        _weaponIcon = transform.Find("WeaponIcon").GetComponent<Image>();
        _loadImage = transform.Find("Load").GetComponent<Image>();
        _loadText = _loadImage.GetComponentInChildren<TMP_Text>();
        _weaponDescrideBack = _weaponIcon.transform.GetChild(0).GetComponent<CanvasGroup>();
        _weaponDescride = _weaponDescrideBack.GetComponentInChildren<TMP_Text>();
        alpahSprite = _weaponIcon.sprite;
        _selectRect = GetComponentInChildren<CanvasGroup>();
        var texts = GetComponentsInChildren<TMP_Text>();
        _soltButton = GetComponentInChildren<Button>();
        _weaponNameText = texts[1];
        _ammunitionText = texts[2];
        _soltButton.onClick.AddListener(OnSelect);
    }
    public void SetWeaponView(FirearmsItem item)
    {
        _weaponIcon.sprite = item.sprite;
        var ammonStr = item.ReserveAmmo == -1 ? "∞" : item.ReserveAmmo.ToString();
        _ammunitionText.text = $"{item.CurrentBullet}/{ammonStr}";
        _weaponNameText.text = item.itemName;
        item.onAmmoChange += OnAmmoChangeEvent;
        _firearm = item;
        _weaponDescride.text = item.describe;
        _weaponDescride.text += $"\n\n<size=8><color=#8FEDFF>射速:{item.ShootTimer}s/发\n装弹:{item.ReloadTimer}s</color></size>";
        item.gameObject.SetActive(false);
        item.onReloadTimerListener += ReloadEffect;
    }
    public FirearmsItem ClearView()
    {
        var result = _firearm;
        ResetLoad();
        _weaponIcon.sprite = alpahSprite;
        if (_firearm != null)
        {
            _firearm.onAmmoChange -= OnAmmoChangeEvent;
            GameInfoController.gameInfoPanelUpdate -= _firearm.Shoot;
            _firearm.onReloadTimerListener -= ReloadEffect;
            GameObject.Destroy(_firearm.gameObject);
            _firearm = null;
        }
        _ammunitionText.text = "";
        _weaponNameText.text = "";
        return result;
        // if (!isSelect)
        //     return;
        // OnOpenClick();
        // ResetLoad();
        // if (_firearm == null)
        //     return;
        // _firearm.gameObject.SetActive(false);
        // _firearm.OnQuitSelect();
        // GameInfoController.gameInfoPanelUpdate -= _firearm.Shoot;
    }
    //TODO: 丢弃效果
    public FirearmsItem ThrowWeapon()
    {
        return ClearView();
    }
    public void ReloadEffect(float value, float maxValue)
    {
        _loadText.text = value.ToString("f2");
        _loadImage.fillAmount = value / maxValue;
        if (value <= 0)
            ResetLoad();
    }
    public void ResetLoad()
    {
        _loadImage.fillAmount = 0;
        _loadText.text = "";
    }
    private void OnAmmoChangeEvent(int current, int reserve)
    {
        var ammonStr = reserve == -1 ? "∞" : reserve.ToString();
        _ammunitionText.text = $"{current}/{ammonStr}";
    }
    public void InitSolt(WeaponManager manager, int i)
    {
        _weaponManager = manager;
        _soltID = i + 1;
        ClearView();
    }
    public void ReSelect()
    {
        isSelect = false;
        if (_firearm != null)
            GameInfoController.gameInfoPanelUpdate -= _firearm.Shoot;
        OnSelect();
    }
    public void OnSelect()
    {
        if (isSelect)
            return;
        _weaponManager.OnSelectIndex(_soltID);
        OnCloseClick();
        if (_firearm == null)
        {
            PacketSendContr.SendWeaponChange(0);
            return;
        }
        PacketSendContr.SendWeaponChange((byte)_firearm.WeaponType);
        _firearm.gameObject.SetActive(true);
        _firearm.OnSelect();
        GameInfoController.gameInfoPanelUpdate += _firearm.Shoot;
    }
    public void OnQuitSelect()
    {
        if (!isSelect)
            return;
        OnOpenClick();
        ResetLoad();
        if (_firearm == null)
            return;
        _firearm.gameObject.SetActive(false);
        _firearm.OnQuitSelect();
        GameInfoController.gameInfoPanelUpdate -= _firearm.Shoot;
    }
    private void OnCloseClick()
    {
        isSelect = true;
        _selectRect.DOFade(1, 0.5f);
    }
    private void OnOpenClick()
    {
        _selectRect.DOFade(0, 0.5f);
        isSelect = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Close();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_firearm == null)
            return;
        _weaponDescrideBack.alpha = 1;
    }
    private void Close()
    {
        _weaponDescrideBack.DOFade(0, 0.5f);
    }
}
