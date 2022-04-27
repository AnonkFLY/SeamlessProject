using System;
using System.Collections;
using System.Collections.Generic;
using AnonSocket.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Player : MonoBehaviour, IHurtable
{
    protected Transform _transform;
    [SerializeField] protected PlayerData playerData;
    [SerializeField] private byte state = 0; //0 stand 1 walk 2 run
    [SerializeField] private PlayerInfoData _infoData = new PlayerInfoData(100, 20, 3);
    protected float speed = 3f;
    public Action<Damage, Player> onDamage;//damageValue,

    [SerializeField] private Vector3 overPos;
    private Vector3 overDir;
    protected Transform plams;
    private SliderView _healthSlider;
    private SliderView _armorSlider;
    private Transform root;
    private Text _nameText;
    [SerializeField] private float rotationSpeed = 1;

    public Transform Transform { get => _transform; set => _transform = value; }
    public PlayerInfoData InfoData { get => _infoData; }
    public byte State { get => state; set => state = value; }

    protected bool isHurt;
    private WaitForSeconds wait = new WaitForSeconds(0.4f);
    public bool isDelay = true;
    public Action onDead;
    private Vector3 addForce;
    private GameObject _currentWeapon;
    internal void OnDeadForce(Vector3 addForce)
    {
        if (addForce == Vector3.zero)
            return;
        this.addForce = addForce;
    }

    public Action onRenascence;
    private CapsuleCollider _collider;
    private Rigidbody _rig;
    private void Awake()
    {
        _nameText = GetComponentInChildren<Text>();
        _rig = GetComponent<Rigidbody>();
        _transform = transform;
        plams = transform.Find("Palm");
        _collider = GetComponent<CapsuleCollider>();
        onDead += () =>
        {
            //_collider.enabled = false; _rig.useGravity = false; 
            _rig.constraints = RigidbodyConstraints.None;
            _rig.AddForce(addForce, ForceMode.Impulse);
//            print("死了");
        };
        onRenascence += () =>
        {
            //print("活了");
            //_collider.enabled = true; _rig.useGravity = true; 
            _rig.velocity = Vector3.zero;
            _transform.eulerAngles = overDir;
            //_transform.rotation = Quaternion.identity;
            _rig.constraints = RigidbodyConstraints.FreezeRotation;
        };
        //rig = GetComponent<Rigidbody>();
        RegisterEvent();
    }
    public void SetWeaponModel(GameObject weapon)
    {
        if (_currentWeapon)
        {
            Destroy(_currentWeapon);
            _currentWeapon = null;
        }
        if (weapon == null)
            return;
        weapon.transform.SetParent(plams);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        _currentWeapon = weapon;

    }
    public void UpdateData(PlayerData data)
    {
        playerData = data;
        if (_nameText)
            _nameText.text = data.GetName();
    }
    private void Update()
    {
        if (state == 1)
            return;
        Move();
    }
    public void ChangePos(Vector3 pos)
    {
        //print(pos);
        _transform.position = pos;
        _rig.velocity = Vector3.zero;
        _transform.eulerAngles = overDir;
    }
    private void LateUpdate()
    {
        if (root == null)
            return;
        root.eulerAngles = Vector3.zero;
    }
    public virtual void Move()
    {
        RotationEffect();
        if (isHurt && isDelay)
            return;
        var overOverPos = overPos + (overPos - transform.position).normalized * 1;
        overOverPos.y = overPos.y;
        var moveDir = overOverPos - _transform.position;
        //var distance = moveDir.sqrMagnitude;
        var overDistance = (overPos - _transform.position).sqrMagnitude;
        if (overDistance < 0.01f)
        {
            _transform.position = overPos;
        }
        else
        {
            var overSpeed = speed * Time.deltaTime * moveDir.normalized;
            if (0.5f < overDistance)
            {
                overSpeed *= 1.6f;
            }
            // if (overSpeed.sqrMagnitude > distance * 2)
            // {
            //     overSpeed *= distance;
            // }
            _transform.Translate(overSpeed, Space.World);
        }

    }
    private void RotationEffect()
    {
        if (overDir == Vector3.zero || state == 1)
            return;
        var value = Vector3.Angle(_transform.localEulerAngles, overDir);
        _transform.localEulerAngles = Vector3.Lerp(_transform.localEulerAngles, overDir, rotationSpeed * Time.deltaTime);
    }
    public void SetState(byte state)
    {
        if (this.state != state)
        {
            switch (state)
            {
                case 0:
                    onRenascence?.Invoke();
                    break;
                case 1:
                    onDead?.Invoke();
                    break;
            }
        }
        this.state = state;
    }
    public virtual void SetPosAndEluer(Vector3 pos, Vector3 dir, byte state)
    {
        SetState(state);
        if (overPos != pos)
            overPos = pos;
        overDir = dir;
        if (Vector3.Distance(pos, _transform.position) > 2)
        {
            _transform.position = pos;
        }
    }
    public void UpdatePlayerStateInfo(float maxArmor, float armor, float maxHealth, float health)
    {
        _infoData.UpdateInfo(maxArmor, armor, maxHealth, health);
    }
    public virtual void RegisterEvent()
    {
        root = transform.Find("Canvas");
        _healthSlider = new SliderView(root.GetChild(0));
        _armorSlider = new SliderView(root.GetChild(1));
        _infoData.health.onValueChange += _healthSlider.SetValue;
        _infoData.armor.onValueChange += _armorSlider.SetValue;
    }

    public void BeHurt(BulletBase bullet)
    {
        isHurt = true;
        StartCoroutine(DelayHurt());
    }
    private IEnumerator DelayHurt()
    {
        yield return wait;
        isHurt = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(overPos, 0.7f);
    }

}
