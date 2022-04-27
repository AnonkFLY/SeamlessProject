using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaEffect : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<SelfPlayer>().onLava = true;
    }
    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<SelfPlayer>().onLava = false;
    }
}
