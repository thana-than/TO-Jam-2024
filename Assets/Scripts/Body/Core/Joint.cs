using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Than.Input;
using System;

public class Joint : MonoBehaviour
{
    public Thorax thorax;
    public Body body => thorax.body;

    public enum Part { LeftArm = -1, RightArm = 1 }
    public Part part;

    public Vector2 Axis_Multiplier => new Vector2(Mathf.Sign((int)part), 1);


    Brain.BoolInput input;

    public Action<bool> input_action;
    public bool input_value => input.value;

    void OnEnable()
    {
        input = body.brain.GetLimbInput(part);
        input.onHeldChange += OnLimbAction;
    }

    void OnDisable()
    {
        input.onHeldChange -= OnLimbAction;
    }

    private void OnLimbAction(bool action)
    {
        this.input_action?.Invoke(action);
    }
}
