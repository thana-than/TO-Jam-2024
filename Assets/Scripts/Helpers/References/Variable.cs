using UnityEngine;

/// <summary>
/// Variable Class.
/// </summary>
public class Variable<T> : ScriptableObject
{
    public virtual T Value => value;

    [SerializeField]
    protected T value;

    //public void SetValue(T value)
    //{
    //    Value = value;
    //}

    //public void SetValue(Variable<T> value)
    //{
    //    Value = value.Value;
    //}
}