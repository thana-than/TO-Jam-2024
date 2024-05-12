using UnityEngine;

public abstract class Limb : MonoBehaviour
{
    protected Joint joint;
    protected Body body => joint.body;
    protected Thorax thorax => joint.thorax;
    public bool invert = false;

    [SerializeField] bool allowActionOutsideOfPlay = false;
    bool init = false;

    public enum Type
    {
        expando,
        wing,
        grip
    }

    public bool Active
    {
        get
        {
            if (!Application.isPlaying || !init)
                return invert;
            return joint.input_value ^ invert;
        }
    }

    protected virtual void OnValidate()
    {
        joint = GetComponentInParent<Joint>();

        if (allowActionOutsideOfPlay)
            OnLimbAction(Active);
    }

    void Awake()
    {
        joint = GetComponentInParent<Joint>();
        init = true;
    }

    protected virtual void OnEnable()
    {
        joint.input_action += PerformAction;
    }

    protected virtual void OnDisable()
    {
        joint.input_action -= PerformAction;
    }

    void PerformAction(bool action) { OnLimbAction(Active); }

    protected virtual void OnLimbAction(bool active) { }
}
