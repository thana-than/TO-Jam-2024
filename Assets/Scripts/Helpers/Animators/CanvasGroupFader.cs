using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFader : MonoBehaviour
{
    CanvasGroup group;

    public float Alpha => group.alpha;

    public float fadeInTime = .5f;
    public float fadeOutTime = .2f;

    public UnityEvent onFadeIn_start;
    public UnityEvent onFadeIn_end;
    public UnityEvent onFadeOut_start;
    public UnityEvent onFadeOut_end;


    void Awake()
    {
        group = GetComponent<CanvasGroup>();
    }

    public void FadeIn() => FadeIn(fadeInTime);
    public void FadeIn(float fadeTime)
    {
        StopAllCoroutines();
        StartCoroutine(FadeInCoroutine(fadeTime));
    }
    public void FadeOut() => FadeOut(fadeOutTime);
    public void FadeOut(float fadeTime)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine(fadeTime));
    }

    IEnumerator FadeInCoroutine(float fadeTime)
    {
        onFadeIn_start?.Invoke();
        if (fadeTime > 0)
        {
            while (group.alpha < 1)
            {
                group.alpha += Time.deltaTime / fadeTime;
                yield return null;
            }
        }

        group.alpha = 1;
        onFadeIn_end?.Invoke();
    }

    IEnumerator FadeOutCoroutine(float fadeTime)
    {
        onFadeOut_start?.Invoke();
        if (fadeTime > 0)
        {
            while (group.alpha > 0)
            {
                group.alpha -= Time.deltaTime / fadeTime;
                yield return null;
            }
        }

        group.alpha = 0;
        onFadeOut_end?.Invoke();
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
}
