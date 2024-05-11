using UnityEngine;

public abstract class Limb : MonoBehaviour
{
    protected Joint joint;
    protected Body body => joint.body;
    public bool invert = false;

    public bool Active
    {
        get
        {
            if (!Application.isPlaying)
                return invert;
            return joint.input_value ^ invert;
        }
    }

    protected virtual void OnValidate()
    {
        joint = GetComponentInParent<Joint>();
    }

    protected virtual void OnEnable()
    {
        joint = GetComponentInParent<Joint>();
        joint.input_action += PerformAction;
    }

    protected virtual void OnDisable()
    {
        joint.input_action -= PerformAction;
    }

    void PerformAction(bool action) { OnLimbAction(Active); }

    protected virtual void OnLimbAction(bool active) { }
}
