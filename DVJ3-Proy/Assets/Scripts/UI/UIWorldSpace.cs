using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldSpace : MonoBehaviour
{
    Transform cam;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("GameplayCamera").transform;
    }
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
