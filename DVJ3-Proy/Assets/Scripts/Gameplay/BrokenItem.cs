using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenItem : MonoBehaviour
{
    [SerializeField] GameObject bottomPiece = null;
    [SerializeField] GameObject topPiece = null;
    Rigidbody bottomPieceRb;
    Rigidbody topPieceRb;
    Collider bottomPieceCol;
    Collider topPieceCol;
    [SerializeField] float forceToSeparate = 0.0f;

    private void OnEnable()
    {
        bottomPieceRb = bottomPiece.GetComponent<Rigidbody>();
        bottomPieceCol = bottomPiece.GetComponent<Collider>();
        topPieceRb = topPiece.GetComponent<Rigidbody>();
        topPieceCol = topPiece.GetComponent<Collider>();
        
        bottomPieceRb.AddForce(-transform.right * forceToSeparate);
        topPieceRb.AddForce(transform.right * forceToSeparate);
        StartCoroutine(SetNotTangible());
    }

    IEnumerator SetNotTangible() //descartar
    {
        yield return new WaitForSeconds(0.5f);
        while (bottomPieceRb.velocity != Vector3.zero || topPieceRb.velocity != Vector3.zero)
        {
            yield return null;
        }
        bottomPieceRb.isKinematic = true;
        bottomPieceCol.isTrigger = true;
        topPieceRb.isKinematic = true;
        topPieceCol.isTrigger = true;
    }
}
