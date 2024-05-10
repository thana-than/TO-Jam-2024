using UnityEngine;

/// <summary>
/// ColorVariable Class.
/// </summary>
[CreateAssetMenu(fileName = "Color_Gradient", menuName = "Variables/Color/Gradient")]
public class ColorVariable_Gradient : ColorVariable
{
    [GradientUsage(true)]
    public Gradient gradient;
    public float time { get; set; } = 0;

    [HideInInspector]
    public override Color Value => Application.isPlaying ? gradient.Evaluate(time) : gradient.Evaluate(0);
}