using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : FadeAndDestroy
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
    Transform itemsHolder = null;
    public enum ItemKind
    { 
        pala,
        bate,
        rama,
        maceta,
        regadera,
        rastrillo,
        herradura,
        hoz,
        balde,
        tijera
    }
    public ItemKind itemKind;
    public enum DirWhenThrown
    {
        forwardUp,
        forwardDown,
        backwardsUp,
        backwardsDown,
    }
    public DirWhenThrown dirToThrow;

    public enum State
    {
        grabbed,
        grabbable,
        midAir,
        broken //despues setear un cd con una coroutine cuando choca contra algo para que no se pueda agarrar por un seg o algo asi
    }
    public State itemState;

    void Start()
    {
        itemsHolder = transform.parent;
        itemState = State.grabbable;
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        mr = GetComponent<MeshRenderer>();
        rotGrabbed = Quaternion.Euler(rotationGrabbed);
        switch (itemKind)
        {
            case ItemKind.pala:
                AkSoundEngine.SetSwitch("Arma_selector", "Pala", gameObject);
                break;
            case ItemKind.bate:
                AkSoundEngine.SetSwitch("Arma_selector", "Bate", gameObject);
                break;
            case ItemKind.rama:
                AkSoundEngine.SetSwitch("Arma_selector", "Rama", gameObject);
                break;
            case ItemKind.maceta:
                AkSoundEngine.SetSwitch("Arma_selector", "Cactus", gameObject);
                break;
            case ItemKind.regadera:
                AkSoundEngine.SetSwitch("Arma_selector", "Regadera", gameObject);
                break;
            case ItemKind.rastrillo:
                AkSoundEngine.SetSwitch("Arma_selector", "Rast_plast", gameObject);
                break;
            case ItemKind.herradura:
                AkSoundEngine.SetSwitch("Arma_selector", "Rast_met", gameObject);
                break;
            case ItemKind.hoz:
                AkSoundEngine.SetSwitch("Arma_selector", "Oz", gameObject);
                break;
            case ItemKind.balde:
                AkSoundEngine.SetSwitch("Arma_selector", "Balde", gameObject);
                break;
            case ItemKind.tijera:
                AkSoundEngine.SetSwitch("Arma_selector", "Guadana", gameObject);
                break;
            default:
                AkSoundEngine.SetSwitch("Arma_selector", "Pala", gameObject);
                break;
        }
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
        coll.isTrigger = true;
        rb.isKinematic = true;
        tangible = false;
        itemState = State.grabbed;
        playerGrabbing = player;
        rb.isKinematic = true;
        transform.localPosition = positionGrabbed;
        transform.localRotation = rotGrabbed;
        //gameObject.layer = LayerMask.NameToLayer("ItemColl"+player.playerNumber);
        visualFeedback.SetActive(false);
        //AkSoundEngine.PostEvent(itemSwitchName, gameObject);
    }

    public void Throw()
    {
        gameObject.layer = LayerMask.NameToLayer("ItemColl" + playerGrabbing.playerNumber);
        itemState = State.midAir;
        rb.isKinematic = false;
        coll.isTrigger = false;
        tangible = true;
        transform.parent = itemsHolder;
        rb.AddForce(playerGrabbing.transform.forward * playerGrabbing.force * toRBPhysics / weight);
        //if (rotationGrabbed == Vector3.zero)
        //{
            rb.AddTorque(playerGrabbing.transform.forward * playerGrabbing.force / weight);
        //}
        //else
        //{
        //    rb.AddTorque(-transform.up * playerGrabbing.force * damageMultiplier / weight); //TEMPORAL, AUTOMATIZAR O LOGRAR UN PIVOT EN OBJETOS QUE NO ESTEN EN IDENTITY (O QUE TE PASEN LOS OBJETOS CON EL PIVOT CORRECTAMENTE)
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (itemState == State.midAir)
        {
            if (!collision.gameObject.CompareTag("Player"))
            {
                SetAsGrabbable();
                itemHitSound();
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
            BreakItem();
        }
    }

    Vector3 GetTorqueDir()
    {
        switch (dirToThrow)
        {
            case DirWhenThrown.forwardUp:
                return -transform.up;
            case DirWhenThrown.forwardDown:
                return transform.up;
            case DirWhenThrown.backwardsUp:
                return transform.up;
            case DirWhenThrown.backwardsDown:
                return -transform.up;
            default:
                return -transform.up;
        }
    }
    

    void BreakItem()
    {
        playerGrabbing.RemoveItemFromAvailable(this);
        visualFeedback.SetActive(false);
        transform.parent = itemsHolder;
        coll.enabled = false;
        mr.enabled = false;
        rb.isKinematic = true;
        itemState = State.broken;
        brokenModel.gameObject.SetActive(true);
        StartFadeAndDestroy();
    }

    public void itemHitSound()
    { 
        AkSoundEngine.PostEvent("Weapon_hit", gameObject);
    }
}
