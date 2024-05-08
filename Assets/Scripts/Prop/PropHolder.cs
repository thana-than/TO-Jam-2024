using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropHolder : MonoBehaviour
{
    public Prop HeldItem => _heldItem;
    [SerializeField] Prop _heldItem;

    public Vector3 HeldLocal => _heldLocalOffset;
    [SerializeField] Vector3 _heldLocalOffset = Vector3.zero;


    void OnEnable()
    {
        Transform child = transform.GetChild(0);
        if (child)
        {
            Prop childItem = child.GetComponent<Prop>();
            if (childItem)
                Hold(childItem);
        }
    }

#if UNITY_EDITOR
    Prop heldItemChangeCheck;
    void OnValidate()
    {
        if (_heldItem != heldItemChangeCheck)
            Hold(_heldItem);
    }
#endif

    public void Hold(Prop item)
    {
        if (_heldItem != item)
            Release();

        _heldItem = item;
        _heldItem.Hold(this);

#if UNITY_EDITOR
        heldItemChangeCheck = _heldItem;
#endif
    }

    public void Release()
    {
        if (!_heldItem)
            return;

        _heldItem.Release();
        _heldItem = null;

#if UNITY_EDITOR
        heldItemChangeCheck = null;
#endif
    }

    public void Throw(Vector3 force)
    {
        if (!_heldItem)
            return;

        Prop item = _heldItem;
        Release();
        item.rb.AddForce(force, ForceMode.Impulse);
    }

}
