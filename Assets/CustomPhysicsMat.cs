
using UnityEngine;

public class CustomPhysicsMat : MonoBehaviour
{
    public BoxCollider2D bounceCollider;

    public float friction = 0;
    public float bounciness = 1;
    PhysicsMaterial2D mat;

    void Start()
    {
        Refresh();
    }

    void Refresh()
    {
        if (!mat)
            mat = new PhysicsMaterial2D();

        mat.friction = friction;
        mat.bounciness = bounciness;
        bounceCollider.sharedMaterial = mat;
    }

    void OnValidate()
    {
        if (!Application.isPlaying)
            return;

        Refresh();
    }
}
