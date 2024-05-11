using UnityEngine;

public class LimbExpander : EvaluatedLimb
{
    public BoxCollider2D boxCollider2D;

    [Space(5)]
    [SerializeField] Vector2 off_offset = Vector2.zero;
    [SerializeField] Vector2 off_size = Vector2.one;

    [Space(5)]
    [SerializeField] Vector2 on_offset = Vector2.zero;
    [SerializeField] Vector2 on_size = Vector2.one;

    public float force_direction_amplifier = 50;
    public float force_surface_amplifier = 50;

    protected override void LimbFixedUpdate(float evaluation)
    {
        (Vector2 offset, Vector2 size) = Evaluate(evaluation);
        boxCollider2D.offset = offset;
        boxCollider2D.size = size;
    }

    (Vector2, Vector2) Evaluate(float evaluation)
    {
        float evalTime = evaluation;
        Vector2 size = Vector2.Lerp(off_size, on_size, evalTime);
        Vector2 offset = Vector2.Lerp(off_offset, on_offset, evalTime);
        return (offset, size);
    }


    private void OnCollisionStay2D(Collision2D other)
    {
        body.rb.AddForce(transform.right * -joint.Axis_Multiplier.x * force_direction_amplifier);
        body.rb.AddForce(other.contacts[0].normal * force_surface_amplifier);
    }

    void OnDrawGizmosSelected()
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;

        (Vector2 o_0, Vector2 s_0) = Evaluate(0);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(o_0, s_0);

        (Vector2 o_1, Vector2 s_1) = Evaluate(1);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(o_1, s_1);

        Gizmos.matrix = oldMatrix;
    }
}
