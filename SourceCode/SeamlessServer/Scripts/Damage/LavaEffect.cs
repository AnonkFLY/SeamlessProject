using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class LavaEffect : MonoBehaviour
{
    [SerializeField] Damage lavaDamage;
    [SerializeField] private List<Player> players;
    private void Awake()
    {
        StartCoroutine(LavaEffectDamage());
    }
    IEnumerator LavaEffectDamage()
    {
        var wait = new WaitForSeconds(1);
        while (true)
        {
            yield return wait;
            players.ForEach(player => player.Info.Damage(lavaDamage));
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        var player = other.GetComponent<Player>();
        if (player)
            players.Add(player);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        var player = other.GetComponent<Player>();
        if (player)
            players.Remove(player);
    }
}
