using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbGrip : Limb
{
    public FixedJoint2D jointToBody;
    public HingeJoint2D grabbyJoint;
    public float radius = .5f;

    void Start()
    {
        jointToBody.autoConfigureConnectedAnchor = true;
        jointToBody.connectedBody = thorax.rb;
        jointToBody.autoConfigureConnectedAnchor = false;
    }

    protected override void OnLimbAction(bool active)
    {
        if (!active)
        {
            grabbyJoint.enabled = false;
            return;
        }

        var hit = Physics2D.CircleCast(transform.TransformPoint(grabbyJoint.anchor), radius, transform.right, .1f);

        Debug.DrawRay(transform.TransformPoint(grabbyJoint.anchor), transform.right * .1f, Color.red, 1);
        if (!hit)
            return;
        Debug.Log(hit);

        grabbyJoint.enabled = true;
        grabbyJoint.autoConfigureConnectedAnchor = true;

        grabbyJoint.connectedBody = hit.rigidbody;
        grabbyJoint.autoConfigureConnectedAnchor = false;

    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.TransformPoint(grabbyJoint.anchor), radius);
    }
}
