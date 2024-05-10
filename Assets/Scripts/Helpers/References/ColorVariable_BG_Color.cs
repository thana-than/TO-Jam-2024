using UnityEngine;

/// <summary>
/// ColorVariable Class.
/// </summary>
[CreateAssetMenu(fileName = "Color_CurrentBackground", menuName = "Variables/Color/BackgroundColor")]
public class ColorVariable_BG_Color : ColorVariable
{
    [HideInInspector]
    public override Color Value => Camera.main.backgroundColor;
}
