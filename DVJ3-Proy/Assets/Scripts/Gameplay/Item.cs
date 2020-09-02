using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Vector3 rotationGrabbed;
    public Vector3 positionGrabbed;
    public float weight;
    Quaternion rotGrabbed;
    Rigidbody rb;
    [HideInInspector]
    public Player playerGrabbing;
    public float damageMultiplier;

    public enum State
    {
        grabbed,
        grabable,
        midAir,
        inCd //despues setear un cd con una coroutine cuando choca contra algo para que no se pueda agarrar por un seg o algo asi
    }
    public State itemState;

    void Start()
    {
        itemState = State.grabable;
        rb = GetComponent<Rigidbody>();
        rotGrabbed = Quaternion.Euler(rotationGrabbed);
    }

    public void SetAsGrabbed(Player player)
    {
        itemState = State.grabbed;
        playerGrabbing = player;
        rb.isKinematic = true;
        transform.localPosition = positionGrabbed;
        transform.localRotation = rotGrabbed;
    }

    public void Throw()
    {
        itemState = State.midAir;
        rb.isKinematic = false;
        transform.parent = null;
        rb.AddForce(transform.forward * playerGrabbing.force / weight);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            playerGrabbing = null;
            itemState = State.grabable;
        }
    }
}
