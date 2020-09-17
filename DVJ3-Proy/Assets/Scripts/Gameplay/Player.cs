using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerNumber;
    public float speed;
    public bool ableToMove;
    public float force;
    public BoxCollider punch;
    public GameObject punchPivot;
    public GameObject swingPivot;
    public int hp;
    public Action<Player> OnDeath;
    Animator animator;
    Rigidbody rb;
    float hor;
    float ver;
    string playerAxisX;
    string playerAxisY;
    string playerAxisHit;
    string playerAxisInteract;
    Item grabbedItem;
    public List<Item> itemsInRange = new List<Item>();
    enum State
    { 
        idle,
        walking,
        punching,
        carrying
    }
    State currentState;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerAxisX = "HorizontalP" + playerNumber;
        playerAxisY = "VerticalP" + playerNumber;
        playerAxisHit = "HitP" + playerNumber;
        playerAxisInteract = "Grab-ThrowP" + playerNumber;
        animator = GetComponent<Animator>();
        currentState = State.idle;
    }

    void Update()
    {
        hor = Input.GetAxis(playerAxisX);
        ver = Input.GetAxis(playerAxisY);
        if (Input.GetButtonDown(playerAxisHit) && currentState!=State.punching)
        {
            if (currentState == State.carrying)
                StartCoroutine(Swing());
            else
                StartCoroutine(Hit());
        }
        if (Input.GetButtonDown(playerAxisInteract))
        {
            if (currentState == State.idle)
            {
                Item closestAvaiableItem = GetClosestAvaiableItem();
                if (closestAvaiableItem)
                {
                    currentState = State.carrying;
                    grabbedItem = GetClosestAvaiableItem();
                    grabbedItem.transform.parent = swingPivot.transform;
                    //grabbedItem.SetAsGrabbed(this);
                    animator.SetBool("carrying", true);
                }
            }
            else if (currentState == State.carrying)
            {
                currentState = State.idle;
                grabbedItem.Throw();
                animator.SetBool("carrying", false);
            }
        }
    }

    private void FixedUpdate()
    {
        if (ableToMove && (hor != 0.0f || ver != 0.0f))
        {
            transform.forward = new Vector3(hor, 0, ver);
            //rb.MovePosition(transform.position + transform.forward.normalized * speed * Time.fixedDeltaTime);
            //transform.position += transform.forward.normalized * speed * Time.fixedDeltaTime;
            transform.position += transform.forward.normalized * speed * Time.fixedDeltaTime;
            
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Player"))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        if (collision.gameObject.CompareTag("Punch"))
        {
            Player hitBy = collision.transform.parent.parent.parent.gameObject.GetComponent<Player>();
            Vector3 dir = transform.position - hitBy.transform.position;
            rb.AddForce(dir.normalized*hitBy.force);
            hp -= (int)(hitBy.force / 2000);
            if (hp <= 0) OnDeath(this);
        }
        if (collision.gameObject.CompareTag("Item"))
        {
            Item hitBy = collision.gameObject.GetComponent<Item>();
            if (hitBy.playerGrabbing && hitBy.playerGrabbing!=this)
            {
                Vector3 dir;
                if (hitBy.itemState == Item.State.midAir)
                {
                    dir = transform.position - hitBy.transform.position;
                    Debug.Log("Golpeado a distancia");
                }
                else
                {
                    dir = transform.position - hitBy.playerGrabbing.transform.position;
                    Debug.Log("Golpeado a melee");
                }
                Vector3 horizontalDir = Vector3.Project(dir, new Vector3(dir.x, 0, dir.z));
                rb.AddForce(horizontalDir.normalized * hitBy.playerGrabbing.force);
                hp -= (int)(hitBy.playerGrabbing.force / 2000 * hitBy.damageMultiplier);
                if (hp <= 0) OnDeath(this);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            Item closeItem = other.GetComponent<Item>();
            itemsInRange.Add(closeItem);
            //Debug.Log("En rango: "+ other.name);
        }
    }

     private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            itemsInRange.Remove(other.GetComponent<Item>());
            //Debug.Log("Ya no en rango: "+ other.name);
        }
    }

    Item GetClosestAvaiableItem()
    {
        int closest = -1;
        for (int i = 0; i < itemsInRange.Count; i++)
        {
            if (itemsInRange[i].playerGrabbing == null)
            {
                closest = i;
                break;
            }
        }
        if (closest == -1) return null;
        for (int i = closest + 1; i < itemsInRange.Count; i++)
        {
            if (!itemsInRange[i].playerGrabbing && Vector3.Distance(itemsInRange[i].transform.position, transform.position) < Vector3.Distance(itemsInRange[closest].transform.position, transform.position))
                closest = i;
        }
        return itemsInRange[closest];
    }

    IEnumerator Swing()
    {
        currentState = State.punching;
        animator.SetTrigger("hit");
        Quaternion origRot = Quaternion.identity;
        Quaternion targetRot = Quaternion.Euler(new Vector3(90, 0, 0));
        float timeToRise = 0.333f;
        float timeToDrop = 0.0333f;
        float t = 0.0f;
        Rigidbody itemRB = grabbedItem.GetComponent<Rigidbody>();
        itemRB.isKinematic = false;
        itemRB.constraints = RigidbodyConstraints.FreezeAll;
        while (t < 1)
        {
            t += Time.deltaTime / timeToRise;
            swingPivot.transform.localRotation = Quaternion.Lerp(origRot, targetRot, t);
            yield return null;
        }
        t = 0.0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToDrop;
            swingPivot.transform.localRotation = Quaternion.Lerp(targetRot, origRot, t);
            yield return null;
        }
        yield return new WaitForSeconds(0.05f);
        itemRB.isKinematic = true;
        itemRB.constraints = RigidbodyConstraints.None;
        currentState = State.carrying;
    }
    IEnumerator Hit()
    {
        float animTime = 0.66f;
        currentState = State.punching;
        animator.SetTrigger("hit");
        yield return new WaitForSeconds(animTime);
        currentState = State.idle;
    }
}
