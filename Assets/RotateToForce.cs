using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToForce : MonoBehaviour
{
    Rigidbody2D rb;
    public float velocity = 1;
    public float rotation = 0;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.rotation = Mathf.MoveTowardsAngle(rb.rotation, rotation, velocity * Time.fixedDeltaTime);
    }
}
