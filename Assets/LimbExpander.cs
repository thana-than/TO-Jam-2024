using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Than.Input;
using System;

public class LimbExpander : MonoBehaviour
{
    Limb limb;

    void OnEnable()
    {
        limb = GetComponentInParent<Limb>();
        limb.interaction += LimbAction;
    }

    void OnDisable()
    {
        limb.interaction -= LimbAction;
    }

    private void LimbAction(bool action)
    {
        Debug.Log(action);
    }
}
