using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbEnabler : Limb
{
    public GameObject disable;
    public GameObject enable;
    protected override void OnLimbAction(bool active)
    {
        base.OnLimbAction(active);
        disable.SetActive(!active);
        enable.SetActive(active);
    }
}
