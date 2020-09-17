﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerNumber;
    public float speed;
    public bool ableToMove;
    public float force;
    public GameObject swingPivot;
    public int hp;
    public Action<PlayerController> OnDeath;
    CharacterController cController;
    Animator animator;
    Rigidbody rb;
    Vector3 dir;
    string playerAxisX;
    string playerAxisY;
    string playerAxisHit;
    string playerAxisInteract;
    Item grabbedItem;
    public List<Item> itemsInRange = new List<Item>();
    float distOnGround = 0.3f;
    [Header("Physics")]
    [SerializeField]
    Vector3 velocity;
    [SerializeField]
    Vector3 momentum;
    [SerializeField]
    float drag;
    [Header("Gravity related")]
    [SerializeField]
    bool isGrounded;
    [SerializeField]
    float baseGravity;
    [SerializeField]
    float gravityAcceleration;
    [SerializeField]
    float currentGravity;
    LayerMask groundLayer;
    float punchTime;
    float swingTime;
    [SerializeField]
    float immunityTime;
    [SerializeField]
    bool immune = false;
    
    enum State
    {
        idle,
        walking,
        punching,
        carrying
    }
    [SerializeField]
    State currentState;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cController = GetComponent<CharacterController>();
        playerAxisX = "HorizontalP" + playerNumber;
        playerAxisY = "VerticalP" + playerNumber;
        playerAxisHit = "HitP" + playerNumber;
        playerAxisInteract = "Grab-ThrowP" + playerNumber;
        animator = GetComponentInChildren<Animator>();
        currentState = State.idle;
        groundLayer = LayerMask.GetMask("Ground");
        currentGravity = baseGravity;
        GetAnimationTimes();
    }

    void Update()
    {
        float hor;
        float ver;
        isGrounded = Physics.CheckSphere(transform.position, distOnGround, groundLayer);
        hor = Input.GetAxis(playerAxisX);
        ver = Input.GetAxis(playerAxisY);
        dir = new Vector3(hor, 0, ver);
        if (Input.GetButtonDown(playerAxisHit) && currentState != State.punching)
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
                    grabbedItem.SetAsGrabbed(this);
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
        if (isGrounded)
        {
            currentGravity = baseGravity;
        }
        else
        {
            currentGravity += gravityAcceleration * Time.fixedDeltaTime;
        }
        velocity = Vector3.zero;
        velocity += momentum;
        if (currentGravity > 0)
        {
            velocity += (new Vector3(0.0f, -currentGravity));
            //ableToMove = false;
        }
        cController.Move(velocity * Time.fixedDeltaTime);
        //else if(!ableToMove)
        //{
        //    ableToMove = true;
        //}
        if (ableToMove && dir!=Vector3.zero)
        {
            transform.forward = dir.normalized; //poner axis raw y un lerp en la rotacion en vez de esta wea y mover hacia dir en vez de forward
            cController.Move(transform.forward * speed * Time.fixedDeltaTime);
        }
        momentum = Vector3.zero;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, distOnGround);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        //if (collision.gameObject.CompareTag("Item"))
        //{
        //    Item hitBy = collision.gameObject.GetComponent<Item>();
        //    Debug.Log("Pego: " + hitBy.gameObject.name + " Agarrado por " + hitBy.playerGrabbing);
        //    Debug.Log("Le pego a: " + this);
        //    Debug.Log(hitBy.playerGrabbing != this);
        //    if (hitBy.playerGrabbing && hitBy.playerGrabbing != this)
        //    {
        //        Debug.Log(collision.gameObject.name);
        //        Vector3 dir;
        //        if (hitBy.itemState == Item.State.midAir)
        //        {
        //            dir = transform.position - hitBy.transform.position;
        //            Debug.Log("Golpeado a distancia");
        //        }
        //        else
        //        {
        //            dir = transform.position - hitBy.playerGrabbing.transform.position;
        //            Debug.Log("Golpeado a melee");
        //        }
        //        Vector3 horizontalDir = Vector3.Project(dir, new Vector3(dir.x, 0, dir.z));
        //        StartCoroutine(Pushed(horizontalDir.normalized * hitBy.playerGrabbing.force));//rb.AddForce(horizontalDir.normalized * hitBy.playerGrabbing.force);
        //        hp -= (int)(hitBy.playerGrabbing.force / 2000 * hitBy.damageMultiplier);
        //        if (hp <= 0) OnDeath(this);
        //    }
        //}
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!immune)
        if (other.gameObject.CompareTag("Punch"))
        {
            Vector3 funElevation = new Vector3 (0.0f, 0.35f);
            PlayerController hitBy = other.transform.parent.parent.parent.parent.gameObject.GetComponent<PlayerController>();
            Vector3 dir = transform.position - hitBy.transform.position;
            //rb.AddForce(dir.normalized * hitBy.force);
            StartCoroutine(Pushed((dir.normalized + funElevation) * hitBy.force));
            hp -= (int)(hitBy.force / 2000);
            //if (hp <= 0) OnDeath(this);
            Debug.Log("le pego, zenioor");
        }
    }

    public void OnItemCollision(Item hitBy)
    {
        if (!immune)
        if (hitBy.playerGrabbing && hitBy.playerGrabbing != this)
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
            StartCoroutine(Pushed(horizontalDir.normalized * hitBy.playerGrabbing.force));//rb.AddForce(horizontalDir.normalized * hitBy.playerGrabbing.force);
            hp -= (int)(hitBy.playerGrabbing.force / 2000 * hitBy.damageMultiplier);
            if (hp <= 0) OnDeath(this);
            StartCoroutine(ImmunityTime());
        }
    }
    IEnumerator Pushed(Vector3 initImpulse)
    {
        Vector3 impulse = initImpulse;
        float t = 0.0f;
        while (impulse != Vector3.zero)
        {
            t += drag * Time.deltaTime;
            impulse = Vector3.Lerp(impulse, Vector3.zero, t);
            momentum += impulse;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Hit()
    {
        currentState = State.punching;
        animator.SetTrigger("hit");
        Debug.Log(punchTime);
        yield return new WaitForSeconds(punchTime);
        Debug.Log("hit");
        currentState = State.idle;
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

    IEnumerator ImmunityTime()
    {
        immune = true;
        yield return new WaitForSeconds(immunityTime);
        immune = false;
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
    void GetAnimationTimes()
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            switch (clips[i].name)
            {
                case "punch":
                    punchTime = clips[i].length;
                    break;
                case "swing":
                    swingTime = clips[i].length;
                    break;
            }
        }
    }
}