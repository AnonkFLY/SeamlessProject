using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SelfPlayer : Player
{
    [SerializeField]
    private LayerMask aimLayer;
    [SerializeField]
    private float timer;
    [SerializeField]
    private Transform _aimTrans;
    private Transform _cameraTrans;
    private Camera _camera;
    private Vector3 _forward;
    private Vector3 _right;
    public bool isInput = true;
    private Vector3 _aimPos;
    private Vector3 _aimDir;
    private bool isRun;
    public bool onLava;
    private GameInfoController gameInfoController;
    private Vector3 _offset;
    private void Start()
    {
        onDead += () => { isInput = false; };
        onRenascence += () => { isInput = true; };
        timer = 1f / 32f;
        _camera = Camera.main;
        _cameraTrans = _camera.transform;
        _offset = _cameraTrans.position - (Vector3.up * 1.5f);
        _forward = _cameraTrans.forward;
        _right = _cameraTrans.right;
        _forward.y = 0;
        _right.y = 0;
        gameInfoController = GameManager.Instance.uiManager.GetUIController<GameInfoController>(UI.BaseClass.UIType.GameInfoPanel);
        ;
        onRenascence += () =>
        {
            InfoData.vigour.ResetValue();
        };
        onDead += () =>
        {
            StartCoroutine(DeadView());
        };
        StartCoroutine(MoveSync());
    }
    private void LateUpdate()
    {
        OnAimUpdate();
    }
    private void OnAimUpdate()
    {
        _aimTrans.position = _aimPos;
        _aimTrans.LookAt(_aimDir);
    }
    private IEnumerator DeadView()
    {
        int i = 5;
        var wait = new WaitForSeconds(1);
        while (i > -1)
        {
            gameInfoController.ShowTitle($"<你被击败了>\n<size=35><color=#939393>{i}</color></size>");
            i--;
            yield return wait;
        }
        gameInfoController.ShowTitle("");
    }

    private IEnumerator MoveSync()
    {
        var wait = new WaitForSeconds(timer);
        while (true)
        {
            if (State == 0)
                PacketSendContr.SendInput(playerData.uid, _transform.position, _transform.eulerAngles, State);
            yield return wait;
        }
    }

    public override void Move()
    {
        _cameraTrans.position = Vector3.Lerp(_cameraTrans.position, _transform.position + _offset, 3 * Time.deltaTime);
        if (!isInput)
            return;
        RayCastAim();
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRun = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRun = false;
        }
        var overSpeed = speed;
        if (onLava)
            overSpeed *= 0.6f;
        if (isRun && !onLava)
        {
            if (InfoData.RemoveVigour(1 * Time.deltaTime))
            {
                overSpeed *= 1.5f;
            }
            else
            {
                isRun = false;
            }
        }
        InfoData.AddVigour(0.5f * Time.deltaTime);
        var dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (dir == Vector2.zero)
            return;
        var overDir = dir.x * _right + dir.y * _forward;
        overDir.Normalize();
        _transform.Translate(overSpeed * Time.deltaTime * overDir, Space.World);
        // if (overDir == Vector3.zero)
        //     return;
        // var rotation = Quaternion.LookRotation(overDir);
        // if (Quaternion.Angle(_transform.rotation, rotation) > 0.1f)
        // {
        //     _transform.rotation = Quaternion.Lerp(_transform.rotation, rotation, 10 * Time.deltaTime);
        // }
    }

    private void RayCastAim()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, float.MaxValue, aimLayer))
        {
            _aimPos = (hit.normal * 0.01f) + hit.point;
            _aimDir = (hit.normal * 0.02f) + hit.point;
            var lookAtPos = hit.point;
            lookAtPos.y = _transform.position.y;
            _transform.LookAt(lookAtPos);
        }
    }
    public override void RegisterEvent()
    {

    }

}
