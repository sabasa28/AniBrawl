using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFadeOut : MonoBehaviour
{
    [SerializeField]
    Image image;
    void Start()
    {
        StartCoroutine(BlinkAndFade());
    }

    IEnumerator BlinkAndFade()
    {
        float t = 0;
        float timeToBlink = 1f;
        int blinkTimesBeforeFade = 3;
        Color visibleCol = new Color(1, 1, 1, 1);
        Color invisibleCol = new Color(1, 1, 1, 0);


        for (int i = 0; i < blinkTimesBeforeFade; i++)
        {
            while (t < 1)
            {
                t += Time.deltaTime / timeToBlink;
                image.color = Color.Lerp(invisibleCol, visibleCol, t);
                yield return null;
            }
            t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / timeToBlink;
                image.color = Color.Lerp(visibleCol, invisibleCol, t);
                yield return null;
            }
            t = 0;
        }
    }
}
