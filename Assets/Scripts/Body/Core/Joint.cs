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
        body.onLimbChanged += OnLimbChanged;
    }



    void OnDisable()
    {
        input.onHeldChange -= OnLimbAction;
        body.onLimbChanged -= OnLimbChanged;
    }

    LimbData GetLimbPrefab(Limb.Type limbType)
    {
        LimbTable table = ServiceLocator.Instance.Get<LimbTable>();
        return table.limbs[limbType];
    }

    void CreateLimb(Limb.Type limbType)
    {
        GameObject.Instantiate(GetLimbPrefab(limbType), transform);
    }

    void TrySwapLimb(Limb.Type limbType)
    {
        LimbData limb = GetComponentInChildren<LimbData>();
        bool limbExists = limb;

        bool limbDestroy = limbExists && limb.type != limbType;

        if (limbDestroy)
            GameObject.Destroy(limb.gameObject);

        if (!limbExists || limbDestroy)
            CreateLimb(limbType);
    }

    void OnLimbChanged()
    {
        if (part == Joint.Part.LeftArm)
        {
            TrySwapLimb(body.left_limb);
        }
        else if (part == Joint.Part.RightArm)
        {
            TrySwapLimb(body.right_limb);
        }
    }

    private void OnLimbAction(bool action)
    {
        this.input_action?.Invoke(action);
    }
}
