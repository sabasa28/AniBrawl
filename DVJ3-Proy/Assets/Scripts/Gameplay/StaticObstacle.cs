using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObstacle : MonoBehaviour
{
    public float pushForce;
    public float damage;
    public enum Kind
    {
        arbusto,
        madera
    }
    public Kind objectKind;
    string sound;

    private void Start()
    {
        switch (objectKind)
        {
            case Kind.arbusto:
                sound = "Play_arbusto";
                break;
            case Kind.madera:
                sound = "Play_madera";
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            AkSoundEngine.PostEvent(sound, gameObject);
            Debug.Log("static sound");
        }
    }
}
