using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Than.Input;

public class Thorax : MonoBehaviour
{
    Brain brain => body.brain;
    public Body body;
    public Rigidbody2D rb;

    float rotation = 0;
    public float offset = -90;
    public float rotationLock = 75;

    void Start()
    {
        rb.centerOfMass = Vector2.zero;
    }

    void FixedUpdate()
    {
        if (brain.Look.value == Vector2.zero)
            return;

        rotation = Mathf.Clamp(brain.Look.value.ToDeg(), body.rb.rotation - rotationLock, body.rb.rotation + rotationLock);
        rb.rotation = rotation + offset;
    }
}
