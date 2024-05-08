using System;
using System.Collections;
using System.Collections.Generic;
using Than.Input;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public Brain brain;
    public PropHolder grip;

    [SerializeField] Vector3 center = Vector3.forward;
    [SerializeField] Vector3 size = Vector3.one;

    int collisionMask;

    void Awake()
    {
        collisionMask = LayerCollisonMask.GetCollisionMask(LayerMask.NameToLayer("Interaction"));
    }

    public float throwForce = 20;
    void OnEnable()
    {
        brain.Throw.onRelease += Throw;
        brain.Interact.onRelease += Interact;
    }

    void OnDisable()
    {
        brain.Throw.onRelease -= Throw;
        brain.Interact.onRelease -= Interact;
    }

    void Throw()
    {
        Vector3 force = transform.forward * throwForce;
        grip.Throw(force);
    }

    private void Interact()
    {
        IInteractable interactable = GetInteractable();
        if (interactable != null)
            interactable.Interact(this);
    }

    Collider[] boxCastResults = new Collider[20];
    IInteractable GetInteractable()
    {
        int len = Physics.OverlapBoxNonAlloc(transform.TransformPoint(center), Vector3.Scale(transform.lossyScale, size) * .5f, boxCastResults, transform.rotation, collisionMask, QueryTriggerInteraction.Collide);

        IInteractable closestValid = null;
        float closestDist = Mathf.Infinity;
        for (int i = 0; i < len; i++)
        {
            IInteractable current = boxCastResults[i].GetComponentInParent<IInteractable>();
            if (current == null)
                continue;

            if (!current.CanInteract(this))
                continue;

            Vector3 closestPoint = boxCastResults[i].ClosestPoint(transform.position);
            float distance = Vector2.Distance(closestPoint, transform.position);
            if (distance >= closestDist)
                continue;

            closestDist = distance;
            closestValid = current;
        }

        return closestValid;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = GetGizmosColor();

        Matrix4x4 prevMatrix = Gizmos.matrix;
        Matrix4x4 rotationMatrix = transform.localToWorldMatrix;
        Gizmos.matrix = rotationMatrix;

        Gizmos.DrawSphere(Vector3.zero, .1f);
        Gizmos.DrawLine(Vector3.zero, center);
        Gizmos.DrawWireCube(center, size);

        //?Double check if I actually have to do this
        Gizmos.matrix = prevMatrix;
    }

    Color GetGizmosColor()
    {
        if (Application.isPlaying)
        {
            if (GetInteractable() != null)
                return Color.green;
        }
        return Color.red;
    }
}
