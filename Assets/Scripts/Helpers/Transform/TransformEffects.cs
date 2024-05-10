using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformEffects : MonoBehaviour
{
    public float snapToPixelsPerUnit = 0;
    Vector3 startPosition = Vector3.zero;
    float push_startTime = Mathf.NegativeInfinity;
    public float push_length = 1;
    public Vector3 push_direction = Vector3.zero;
    public AnimationCurve push_curve = AnimationCurve.Linear(0, 0, 1, 0);

    float shake_startTime = Mathf.NegativeInfinity;
    public float shake_length = 1;
    public float shake_frequency = 1;
    public Vector3 shake_axisMultiplier = Vector3.one;
    public AnimationCurve shake_curve = AnimationCurve.Linear(0, 1, 1, 0);

    public bool animationActive { get; private set; }

    //static float CorrectedTime => Time.time % 3000;

    public float fatigue_buildUp = 1;
    public float fatigue_comeDown = 1;
    public AnimationCurve fatigue_curve = AnimationCurve.Constant(0, 0, 0);
    float fatigue_current = 0;

    float effectTime = 0;
    const float TIME_MODULO = 3000;
    public bool rumbleOnEffect = false;


    void Awake()
    {
        startPosition = transform.localPosition;
    }

    public void Stop()
    {
        effectTime = 0;
        transform.localPosition = startPosition;
        push_startTime = Mathf.NegativeInfinity;
        shake_startTime = Mathf.NegativeInfinity;
        fatigue_current = 0;
    }

    void OnDisable()
    {
        Stop();
    }

    public void Push()
    {
        push_startTime = effectTime;
        if (rumbleOnEffect)
            Rumbler.Instance.BothRumbleSoft(shake_length);
    }
    public void Push(Vector3 direction)
    {
        push_direction = direction;
        Push();
    }

    public void Push(Vector3 direction, float length)
    {
        push_length = length;
        Push(direction);
    }

    public void Push(Vector3 direction, float length, AnimationCurve curve)
    {
        push_curve = curve;
        Push(direction, length);
    }

    public void Shake()
    {
        shake_startTime = effectTime;
        if (rumbleOnEffect)
            Rumbler.Instance.BothRumbleSoft(shake_length);
    }

    public void Shake(float length)
    {
        shake_length = length;
        Shake();
    }

    public void Shake(float length, AnimationCurve curve)
    {
        shake_curve = curve;
        Shake(length);
    }

    public void Shake(float length, AnimationCurve curve, float frequency)
    {
        shake_frequency = frequency;
        Shake(length, curve);
    }

    public void Shake(float length, AnimationCurve curve, float frequency, Vector3 axisMultiplier)
    {
        shake_axisMultiplier = axisMultiplier;
        Shake(length, curve, frequency);
    }


    void LateUpdate()
    {
        effectTime += Time.deltaTime;
        if (effectTime > TIME_MODULO)
        {
            effectTime %= TIME_MODULO;
            if (push_startTime > Mathf.NegativeInfinity) push_startTime %= TIME_MODULO;
            if (shake_startTime > Mathf.NegativeInfinity) shake_startTime %= TIME_MODULO;
        }

        float fatigue_multiplier = 1 - fatigue_curve.Evaluate(fatigue_current);

        Vector3 push_offset = Vector3.zero;
        if (push_startTime > Mathf.NegativeInfinity && push_startTime + push_length > effectTime)
        {
            float push_time = effectTime - push_startTime;
            float push_percent = push_curve.Evaluate(Mathf.Clamp01(push_time / push_length));
            push_offset = push_direction * push_percent * fatigue_multiplier;
        }
        else
            push_startTime = Mathf.NegativeInfinity;

        Vector3 shakeOffset = Vector3.zero;
        if (shake_startTime > Mathf.NegativeInfinity && shake_startTime + shake_length > effectTime)
        {
            float shake_time = effectTime - shake_startTime;

            Vector3 noise = new Vector3(
                Mathf.PerlinNoise(transform.localPosition.x, effectTime * shake_frequency) * 2 - 1,
                Mathf.PerlinNoise(effectTime * shake_frequency, transform.localPosition.y) * 2 - 1,
                Mathf.PerlinNoise(transform.localPosition.z, (effectTime + shake_length) * shake_frequency) * 2 - 1);

            Vector3 shakeRandom = noise * shake_curve.Evaluate(Mathf.Clamp01(shake_time / shake_length));
            shakeOffset = new Vector3(shakeRandom.x * shake_axisMultiplier.x, shakeRandom.y * shake_axisMultiplier.y, shakeRandom.z * shake_axisMultiplier.z) * fatigue_multiplier;
        }
        else
            shake_startTime = Mathf.NegativeInfinity;

        animationActive = push_offset != Vector3.zero || shakeOffset != Vector3.zero;
        transform.localPosition = GetSnapped(startPosition + push_offset + shakeOffset, snapToPixelsPerUnit);

        FatigueUpdate();
    }

    void FatigueUpdate()
    {
        float fatigue_change = animationActive ? fatigue_buildUp : -fatigue_comeDown;
        fatigue_current = Mathf.Clamp01(fatigue_current + fatigue_change * Time.deltaTime);
    }

    static Vector3 GetSnapped(Vector3 vector, float pixelsPerUnit)
    {
        if (pixelsPerUnit <= 0)
            return vector;

        Vector3 pixelPos = Vector3Int.RoundToInt(vector * pixelsPerUnit);
        return pixelPos / pixelsPerUnit;
    }
}
