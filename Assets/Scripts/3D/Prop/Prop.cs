using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Prop : MonoBehaviour, IInteractable
{
    public Rigidbody rb => _rb;
    [HideInInspector][SerializeField] Rigidbody _rb;

    public PropHolder currentHolder { get; private set; }

    void OnValidate()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Hold(PropHolder holder)
    {
        if (currentHolder && holder != currentHolder)
            currentHolder.Release();

        currentHolder = holder;
        transform.parent = holder.transform;
        transform.localPosition = holder.HeldLocal;
        transform.localRotation = Quaternion.identity;
        SetPhysicsActive(false);
    }

    public void Release()
    {
        currentHolder = null;
        transform.parent = null;
        SetPhysicsActive(true);
    }

    void SetPhysicsActive(bool active)
    {
        rb.velocity = Vector3.zero;
        rb.isKinematic = !active;
    }

    public bool CanInteract(Interactor interactor)
    {
        return !currentHolder && interactor.grip;
    }

    public void Interact(Interactor interactor)
    {
        interactor.grip.Hold(this);
    }

    public bool IsPhysicsActive => rb.isKinematic;
}
