using System;
using UnityEngine;

/// <summary>
/// Vector2Reference Class.
/// </summary>
[Serializable]
public class Vector2Reference : Reference<Vector2, Vector2Variable>
{
    public Vector2Reference(Vector2 Value) : base(Value) { }
    public Vector2Reference() { }
}