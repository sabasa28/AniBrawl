using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObstacle : FadeAndDestroy
{
    [SerializeField] GameObject brokenModel;
    [SerializeField] float fallenPositionY;
    public float pushForce;
    public float damage;
    bool canDamage = true;
    Rigidbody rb;
    Collider coll;
    MeshRenderer mr;

    public enum KindOfObj
    {
        manzana,
        huevo
    }
    public KindOfObj objectKind;
    string sound;

    void Start()
    {
        switch (objectKind)
        {
            case KindOfObj.manzana:
                sound = "Play_manzana";
                break;
            case KindOfObj.huevo:
                sound = "Play_huevo";
                break;
        }
        mr = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canDamage && other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(SetAsNotFalling());
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
            StartFadeAndDestroy();
            AkSoundEngine.PostEvent(sound, gameObject);
        }
    }

    IEnumerator SetAsNotFalling()
    {
        yield return new WaitForEndOfFrame();
        canDamage = false;
    }
}
