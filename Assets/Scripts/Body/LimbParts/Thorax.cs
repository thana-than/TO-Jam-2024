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

    void Update()
    {
        if (brain.Look.value == Vector2.zero)
            return;

        rotation = Mathf.Clamp(brain.Look.value.ToDeg(), transform.parent.rotation.z - rotationLock, transform.parent.rotation.z + rotationLock);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotation + offset));
    }
}
