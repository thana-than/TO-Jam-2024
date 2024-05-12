using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LimbTransformLerp : EvaluatedLimb
{

    public bool use_position = false;
    public bool use_rotation = false;
    public bool use_scale = false;

    [System.Serializable]
    struct State
    {
        public Vector3 position;
        public float rotation;
        public Vector3 scale;

        public static State Lerp(State a, State b, float evaluation)
        {
            return new State
            {
                position = Vector3.Lerp(a.position, b.position, evaluation),
                rotation = Mathf.LerpAngle(a.rotation, b.rotation, evaluation),
                scale = Vector3.Lerp(a.scale, b.scale, evaluation)
            };
        }
    }

    [SerializeField] State off_state;
    [SerializeField] State on_state;

    protected override void LimbUpdate(float evaluation)
    {
        base.LimbUpdate(evaluation);

        Evaluate(evaluation);
    }

    void Evaluate(float evaluation)
    {
        State s = State.Lerp(off_state, on_state, evaluation);

        if (use_position)
            transform.localPosition = s.position;
        if (use_rotation)
            transform.localRotation = Quaternion.Euler(0, 0, s.rotation);
        if (use_scale)
            transform.localScale = s.scale;
    }
}
