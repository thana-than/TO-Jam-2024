using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageFader : MonoBehaviour
{
    Image img;

    public float fadeInTime = .5f;
    public float fadeOutTime = .2f;

    public bool fadeInOnEnable = false;

    void Awake()
    {
        img = GetComponent<Image>();
    }

    void OnEnable()
    {
        Color c = img.color;
        c.a = 0;
        img.color = c;

        if (fadeInOnEnable)
            FadeIn();
    }

    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeIn(fadeInTime));
    }
    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut(fadeOutTime));
    }

    IEnumerator FadeIn(float fadeTime)
    {
        Color c = img.color;

        if (fadeTime > 0)
        {

            while (c.a < 1)
            {
                c.a += Time.deltaTime / fadeTime;
                img.color = c;
                yield return null;
            }
        }

        c.a = 1;
        img.color = c;
    }

    IEnumerator FadeOut(float fadeTime)
    {
        Color c = img.color;

        if (fadeTime > 0)
        {

            while (c.a > 0)
            {
                c.a -= Time.deltaTime / fadeTime;
                img.color = c;
                yield return null;
            }
        }

        c.a = 0;
        img.color = c;
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
}
