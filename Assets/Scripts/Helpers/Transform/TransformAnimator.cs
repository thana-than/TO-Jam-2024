using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TransformAnimator : MonoBehaviour
{
    [System.Serializable]
    public struct Sequence
    {
        public Vector3 position;
        public Vector3 scale;
        public float time;
        public AnimationCurve curve;
        public UnityEvent onSequenceStart;
        public UnityEvent onSequenceEnd;
    }

    public float snapToPixelsPerUnit = 0;
    public Sequence[] sequences;

    public void PlaySequence(int index)
    {
        if (!this.isActiveAndEnabled)
            return;

        StopAllCoroutines();
        StartCoroutine(AnimateSequence(transform, sequences[index]));
    }

    public void JumpToSequence(int index)
    {
        if (!this.isActiveAndEnabled)
            return;

        StopAllCoroutines();
        SetToSequence(transform, sequences[index]);
        // sequences[index].onSequenceStart?.Invoke();
        // transform.localPosition = sequences[index].position;
        // transform.localScale = sequences[index].scale;
        // sequences[index].onSequenceEnd?.Invoke();
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public static void SetToSequence(Transform transform, Sequence sequence, float pixelsPerUnit = 0)
    {
        sequence.onSequenceStart?.Invoke();
        transform.localPosition = GetSnapped(sequence.position, pixelsPerUnit);
        transform.localScale = GetSnapped(sequence.scale, pixelsPerUnit);
        sequence.onSequenceEnd?.Invoke();
    }

    public static IEnumerator AnimateSequence(Transform transform, Sequence sequence, float pixelsPerUnit = 0)
    {
        Vector3 startPosition = GetSnapped(transform.localPosition, pixelsPerUnit);
        Vector3 startScale = GetSnapped(transform.localScale, pixelsPerUnit);
        sequence.onSequenceStart?.Invoke();
        for (float t = 0; t < 1; t += Time.deltaTime / sequence.time)
        {
            transform.localPosition = GetSnapped(Vector3.Lerp(startPosition, sequence.position, sequence.curve.Evaluate(t)), pixelsPerUnit);
            transform.localScale = GetSnapped(Vector3.Lerp(startScale, sequence.scale, sequence.curve.Evaluate(t)), pixelsPerUnit);
            yield return null;
        }

        transform.localPosition = GetSnapped(sequence.position, pixelsPerUnit);
        transform.localScale = GetSnapped(sequence.scale, pixelsPerUnit);
        sequence.onSequenceEnd?.Invoke();
    }

    static Vector3 GetSnapped(Vector3 vector, float pixelsPerUnit)
    {
        if (pixelsPerUnit <= 0)
            return vector;

        Vector3 pixelPos = Vector3Int.RoundToInt(vector * pixelsPerUnit);
        return pixelPos / pixelsPerUnit;
    }
}
