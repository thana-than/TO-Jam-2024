using System;
using Than.Input;
using UnityEngine;

public class Body : MonoBehaviour
{
    public Brain brain;
    public Rigidbody2D rb;
    Vector2 centerOfMass = Vector2.zero;

    public Limb.Type left_limb;
    public Limb.Type right_limb;

    public Action onLimbChanged;

    void Start()
    {
        rb.centerOfMass = centerOfMass;
        onLimbChanged?.Invoke();
    }
    void OnValidate()
    {
        rb.centerOfMass = centerOfMass;
        onLimbChanged?.Invoke();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.TransformPoint(rb.centerOfMass), .1f);
    }
}
