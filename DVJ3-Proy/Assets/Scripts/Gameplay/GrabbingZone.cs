using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbingZone : MonoBehaviour
{
    public PlayerController player;
    private void FixedUpdate()
    {
        transform.position = player.transform.position;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            Item closeItem = other.GetComponent<Item>();
            player.itemsInRange.Add(closeItem);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            player.itemsInRange.Remove(other.GetComponent<Item>());
        }
    }
}
