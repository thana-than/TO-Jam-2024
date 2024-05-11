using Than.Input;
using UnityEngine;

public class Body : MonoBehaviour
{
    public Brain brain;
    public Rigidbody2D rb;
    Vector2 centerOfMass = Vector2.zero;

    void Start()
    {
        rb.centerOfMass = centerOfMass;
    }
    void OnValidate()
    {
        rb.centerOfMass = centerOfMass;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.TransformPoint(rb.centerOfMass), .1f);
    }
}
