using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerWS : MonoBehaviour
{
    [SerializeField] Image image = null;

    private void Start()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
    }
    public void Animate()
    {
        StartCoroutine(BlinkAndFade());
    }

    IEnumerator BlinkAndFade()
    {
        float t = 0;
        float timeToBlink = 1f;
        int blinkTimesBeforeFade = 2;
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
