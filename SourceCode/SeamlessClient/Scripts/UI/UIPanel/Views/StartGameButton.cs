using System.Collections;
using System.Collections.Generic;
using AnonSocket.Data;
using UnityEngine;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            var packet = new PacketBase(-24, 8);
            ClientHub.Instance.SendPacket(packet);
            gameObject.SetActive(false);
        });
    }
}
