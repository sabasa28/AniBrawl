using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObstacle : MonoBehaviour
{
    [SerializeField] GameObject brokenModel;
    [SerializeField] float fallenPositionY;
    public float pushForce;
    public float damage;
    bool canDamage = true;
    Rigidbody rb;
    Collider coll;
    MeshRenderer mr;
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canDamage && other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(SetAsNotFalling());
            Debug.Log("hit player falling obj");
        }
        if (other.gameObject.CompareTag("Floor"))
        {
            rb.isKinematic = true;
            coll.enabled = false;
            transform.position = new Vector3(transform.position.x, fallenPositionY, transform.position.z);
            transform.rotation = Quaternion.identity;
            mr.enabled = false;
            brokenModel.SetActive(true);
            canDamage = false;
        }
    }

    IEnumerator SetAsNotFalling()
    {
        yield return new WaitForEndOfFrame();
        canDamage = false;
    }
}
