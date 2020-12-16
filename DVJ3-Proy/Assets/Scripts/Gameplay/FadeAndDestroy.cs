using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAndDestroy : MonoBehaviour
{
    float timeFading = 5.0f;
    float timeBeforeFading = 5.0f;

    protected void StartFadeAndDestroy()
    {
        StartCoroutine(FadeObjectAndChildsAndDestroy());

    }
    protected IEnumerator FadeObjectAndChildsAndDestroy()
    {
        yield return new WaitForSeconds(timeBeforeFading);
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        MeshRenderer mr;
        TryGetComponent(out mr);
        float t = 0.0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeFading;
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material.color = Color.Lerp(meshRenderers[i].material.color, new Color(meshRenderers[i].material.color.r, meshRenderers[i].material.color.g, meshRenderers[i].material.color.b, 0), t);
            }
            if (mr) mr.material.color = Color.Lerp(mr.material.color, new Color(mr.material.color.r, mr.material.color.g, mr.material.color.b, 0), t);
            yield return null;
        }
        Destroy(gameObject);
    }
}
