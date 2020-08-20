using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator animator; 
    public int playerNumber;
    public float speed;
    public bool ableToMove;
    float hor;
    float ver;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("hit");
        }
    }

    private void FixedUpdate()
    {
        if (ableToMove && (hor != 0.0f || ver != 0.0f))
        {
            transform.forward = new Vector3(hor, 0, ver);
            transform.position += transform.forward.normalized * speed * Time.deltaTime;
        }
    }
}
