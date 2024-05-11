using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Than.Input;
using System;

public class Limb : MonoBehaviour
{
    public Brain brain;
    public enum Part { LeftArm = -1, RightArm = 1 }
    public Part part;

    Brain.BoolInput input;

    public Action<bool> interaction;

    void OnEnable()
    {
        input = brain.GetLimbInput(part);
        input.onHeldChange += OnLimbAction;
    }

    void OnDisable()
    {
        input.onHeldChange -= OnLimbAction;
    }

    private void OnLimbAction(bool action)
    {
        interaction?.Invoke(action);
    }
}
