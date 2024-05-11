using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbForce : Limb
{
    public Vector2 force = new Vector2(-1, 10);

    protected override void OnLimbAction(bool active)
    {
        if (active)
        {
            Vector2 f = transform.TransformVector(force * joint.Axis_Multiplier);
            body.rb.AddForce(f, ForceMode2D.Impulse);
        }
    }
}
