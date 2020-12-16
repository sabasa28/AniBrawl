using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactCollider : MonoBehaviour
{
    PlayerController pController;
    void Start()
    {
        pController = transform.parent.GetComponent<PlayerController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            Item hitBy = collision.gameObject.GetComponent<Item>();
            pController.OnItemCollision(hitBy);
        }
        if (collision.gameObject.CompareTag("StaticObstacle"))
        {
            StaticObstacle hitBy = collision.gameObject.GetComponent<StaticObstacle>();
            pController.OnStaticObstacleCollision(hitBy);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Item"))
        {
            Item hitBy = other.gameObject.GetComponent<Item>();
            pController.OnItemCollision(hitBy);
        }
        if (other.gameObject.CompareTag("FallingObstacle"))
        {
            Debug.Log("Impact falling");
            FallingObstacle hitBy = other.gameObject.GetComponent<FallingObstacle>();
            pController.OnFallingObstacleCollision(hitBy);
        }
    }
}
