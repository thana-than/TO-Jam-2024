using System;

/// <summary>
/// IntReference Class.
/// </summary>
[Serializable]
public class IntReference : Reference<int, IntVariable>
{
    public IntReference(int Value) : base(Value) { }
    public IntReference() { }
}
