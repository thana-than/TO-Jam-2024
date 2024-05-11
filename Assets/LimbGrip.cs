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
        jointToBody.connectedAnchor = joint.transform.localPosition;
        jointToBody.connectedBody = thorax.rb;
    }

    protected override void OnLimbAction(bool active)
    {
        if (!active)
        {
            grabbyJoint.enabled = false;
            return;
        }

        //TODO layermasking
        var hit = Physics2D.CircleCast(transform.TransformPoint(grabbyJoint.anchor), radius, transform.right, .1f);

        Debug.DrawRay(transform.TransformPoint(grabbyJoint.anchor), transform.right * .1f, Color.red, 1);
        if (!hit)
            return;

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
