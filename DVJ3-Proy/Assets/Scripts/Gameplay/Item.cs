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
    Collider coll;
    float toRBPhysics = 100;
    public PlayerController playerGrabbing;
    public float damageMultiplier;
    bool tangible = false;

    public enum State
    {
        grabbed,
        grabbable,
        midAir,
        inCd //despues setear un cd con una coroutine cuando choca contra algo para que no se pueda agarrar por un seg o algo asi
    }
    public State itemState;

    void Start()
    {
        itemState = State.grabbable;
        rb = GetComponentInChildren<Rigidbody>();
        coll = GetComponentInChildren<Collider>();
        rotGrabbed = Quaternion.Euler(rotationGrabbed);
    }

    private void Update()
    {
        if (tangible && itemState == State.grabbable && rb.velocity == Vector3.zero)
        {
            coll.isTrigger = true;
            rb.isKinematic = true;
            tangible = false;
        }

    }
    public void SetAsGrabbed(PlayerController player)
    {
        itemState = State.grabbed;
        playerGrabbing = player;
        rb.isKinematic = true;
        transform.localPosition = positionGrabbed;
        transform.localRotation = rotGrabbed;
        gameObject.layer = LayerMask.NameToLayer("ItemColl"+player.playerNumber);
    }

    public void Throw()
    {
        itemState = State.midAir;
        rb.isKinematic = false;
        coll.isTrigger = false;
        tangible = true;
        transform.parent = null;
        rb.AddForce(transform.forward * playerGrabbing.force * toRBPhysics / weight);
        if (rotationGrabbed == Vector3.zero)
        {
            rb.AddTorque(transform.right * playerGrabbing.force / weight);
        }
        else
        {
            rb.AddTorque(-transform.up * playerGrabbing.force / weight); //TEMPORAL, AUTOMATIZAR O LOGRAR UN PIVOT EN OBJETOS QUE NO ESTEN EN IDENTITY (O QUE TE PASEN LOS OBJETOS CON EL PIVOT CORRECTAMENTE)
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (itemState == State.midAir)
        {
            if (!collision.gameObject.CompareTag("Player"))
            {
                gameObject.layer = LayerMask.NameToLayer("Item");
                playerGrabbing = null;
                itemState = State.grabbable;
            }
        }
    }

    public void SetAsGrabbable()
    {
        gameObject.layer = LayerMask.NameToLayer("Item");
        playerGrabbing = null;
        itemState = State.grabbable;
    }
}
