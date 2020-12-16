using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireHazzard : MonoBehaviour
{
    [SerializeField] Animator[] fireAnimators;
    [SerializeField] float timeOn;
    public float damagePerHit;
    Collider col;
    private void Start()
    {
        col = GetComponent<Collider>();
        AkSoundEngine.PostEvent("Play_fuego", gameObject);
        StartCoroutine(TurnOnAndOff());
    }
    IEnumerator TurnOnAndOff()
    {
        float timeBeforeDestroying = 1.0f;
        yield return new WaitForSeconds(timeOn);
        for (int i = 0; i < fireAnimators.Length; i++)
        {
            fireAnimators[i].SetTrigger("TurnOff");
        }
        col.enabled = false;
        yield return new WaitForSeconds(timeBeforeDestroying);
        Destroy(gameObject);
    }
}
