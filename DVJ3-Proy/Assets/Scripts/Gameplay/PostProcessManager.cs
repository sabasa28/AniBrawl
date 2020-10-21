using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessManager : MonoBehaviour
{
    [SerializeField] float timeToRemove = 0;
    PostProcessVolume postProcess;
    ChromaticAberration cAberr = null;

    private void Awake()
    {
        postProcess = GetComponent<PostProcessVolume>();
        postProcess.profile.TryGetSettings(out cAberr);
    }

    public void StartRemovingCAberration()
    {
        StartCoroutine(RemoveBlurOverTime());
    }

    IEnumerator RemoveBlurOverTime()
    {
        float t = 0.0f;
        float origIntensity = cAberr.intensity;

        while (t < 1.0f)
        {
            t += Time.deltaTime / timeToRemove;
            cAberr.intensity.value = Mathf.Lerp(origIntensity, 0, t);
            yield return null;
        }
        cAberr.active = false;
    }
}
