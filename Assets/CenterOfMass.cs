using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CenterOfMass : MonoBehaviour
{
    Rigidbody2D rb;
    public Vector2 centerOfMass = Vector2.zero;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.centerOfMass = centerOfMass;
    }

    void OnValidate()
    {
        if (!rb)
            rb = GetComponent<Rigidbody2D>();
        rb.centerOfMass = centerOfMass;
    }

    void OnDrawGizmosSelected()
    {
        if (!rb)
            rb = GetComponent<Rigidbody2D>();
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.TransformPoint(rb.centerOfMass), .05f);
    }
}
