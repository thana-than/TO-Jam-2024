using System;

/// <summary>
/// FloatReference Class.
/// </summary>
[Serializable]
public class FloatReference : Reference<float, FloatVariable>
{
    public FloatReference(float Value) : base(Value) { }
    public FloatReference() { }
}
