using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerNumber;
    public float speed;
    Vector3 dir;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float hor;
        float ver;
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");
        dir = new Vector3(hor, 0, ver);
        Vector3 lastPos = transform.position;
        transform.position += dir.normalized * speed * Time.deltaTime;
    }
    private void FixedUpdate()
    {
    }
}
