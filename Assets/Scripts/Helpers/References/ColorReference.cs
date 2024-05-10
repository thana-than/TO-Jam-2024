using System;

using UnityEngine;

/// <summary>
/// ColorReference Class.
/// </summary>
[Serializable]
public class ColorReference : Reference
{
    public bool UseConstant = true;

    [ColorUsage(false, true)]
    public Color ConstantValue = default;

    public ColorVariable Variable;

    public ColorReference() { }
    public ColorReference(Color value)
    {
        UseConstant = true;
        ConstantValue = value;
    }

    public Color Value
    {
        get { return UseConstant || Variable == null ? ConstantValue : Variable.Value; }
    }

    public static implicit operator Color(ColorReference Reference)
    {
        return Reference.Value;
    }

    public static implicit operator ColorReference(Color Value)
    {
        return new ColorReference(Value);
    }
}
/*
[Serializable]
public class ColorReference : Reference<Color, ColorVariable>
{
    public ColorReference(Color Value) : base(Value) { }
    public ColorReference() { }
}
*/