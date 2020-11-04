using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Vector3 rotationGrabbed;
    public Vector3 positionGrabbed;
    public float weight;
    public int durability;
    Quaternion rotGrabbed;
    Rigidbody rb;
    Collider coll;
    MeshRenderer mr;
    float toRBPhysics = 100;
    public PlayerController playerGrabbing;
    public float damageMultiplier;
    bool tangible = false;
    [SerializeField] GameObject brokenModel = null;
    [SerializeField] GameObject visualFeedback = null;
    public enum DirToThrow
    {
        forward,
        backwards,
        right,
        left,
        up,
        down
    }
    public DirToThrow dirToThrow;

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
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        mr = GetComponent<MeshRenderer>();
        rotGrabbed = Quaternion.Euler(rotationGrabbed);
    }

    private void Update()
    {
        if (tangible && itemState == State.grabbable && rb.velocity == Vector3.zero && rb.isKinematic==false)
        {
            coll.isTrigger = true;
            rb.isKinematic = true;
            tangible = false;
            visualFeedback.SetActive(true);
        }
    }
    public void SetAsGrabbed(PlayerController player)
    {
        itemState = State.grabbed;
        playerGrabbing = player;
        rb.isKinematic = true;
        transform.localPosition = positionGrabbed;
        transform.localRotation = rotGrabbed;
        //gameObject.layer = LayerMask.NameToLayer("ItemColl"+player.playerNumber);
        visualFeedback.SetActive(false);
    }

    public void Throw()
    {
        gameObject.layer = LayerMask.NameToLayer("ItemColl" + playerGrabbing.playerNumber);
        itemState = State.midAir;
        rb.isKinematic = false;
        coll.isTrigger = false;
        tangible = true;
        transform.parent = null;
        rb.AddForce(playerGrabbing.transform.forward * playerGrabbing.force * toRBPhysics / weight);
        if (rotationGrabbed == Vector3.zero)
        {
            rb.AddTorque(transform.right * playerGrabbing.force / weight);
        }
        else
        {
            rb.AddTorque(-transform.up * playerGrabbing.force * damageMultiplier / weight); //TEMPORAL, AUTOMATIZAR O LOGRAR UN PIVOT EN OBJETOS QUE NO ESTEN EN IDENTITY (O QUE TE PASEN LOS OBJETOS CON EL PIVOT CORRECTAMENTE)
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (itemState == State.midAir)
        {
            if (!collision.gameObject.CompareTag("Player"))
            {
                SetAsGrabbable();
            }
        }
    }

    public void SetAsGrabbable()
    {
        gameObject.layer = LayerMask.NameToLayer("Item");
        playerGrabbing = null;
        itemState = State.grabbable;
    }

    public void GetDamaged()
    {
        durability--;
        if (durability <= 0)
        {
            GetBroken();
        }
    }

    /* ESTO SE USABA PARA TENER EL FORWARD DEL OBJETO PERO YA NO, PUEDE QUE LO NECESITE PARA EL TORQUE, NOT SURE
    Vector3 GetCurrentRelativeForward()
    {
        switch (dirToThrow)
        {
            case DirToThrow.forward:
                return transform.forward;
            case DirToThrow.backwards:
                return -transform.forward;
            case DirToThrow.right:
                return transform.right;
            case DirToThrow.left:
                return -transform.right;
            case DirToThrow.up:
                return transform.up;
            case DirToThrow.down:
                return -transform.up;
            default:
                return transform.forward;
        }
    }
    */

    void GetBroken()
    {
        visualFeedback.SetActive(false);
        transform.parent = null;
        coll.enabled = false;
        mr.enabled = false;
        rb.isKinematic = true;
        brokenModel.gameObject.SetActive(true);
    }
}
