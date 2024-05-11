using UnityEngine;

public class LimbFlap : EvaluatedLimb
{
    public float startRot = 45;
    public float endRot = -45;

    protected override void LimbFixedUpdate(float evaluation)
    {
        Evaluate(evaluation);
    }

    void Evaluate(float evaluation)
    {
        float rot = Mathf.LerpAngle(startRot, endRot, evaluation);
        transform.localRotation = Quaternion.Euler(0, 0, rot);
    }

    void OnDrawGizmosSelected()
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.parent.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector2.zero, UMath.DegreeToVector2(startRot) * 5);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(Vector2.zero, UMath.DegreeToVector2(endRot) * 5);

        Gizmos.matrix = oldMatrix;
    }
}
