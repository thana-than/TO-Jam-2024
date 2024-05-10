using UnityEngine;

/// <summary>
/// ColorVariable Class.
/// </summary>
[CreateAssetMenu(fileName = "New Color Variable", menuName = "Variables/Color/Value")]
public class ColorVariable : ScriptableObject //Variable<Color>
{
    public virtual Color Value => value;

    [SerializeField][ColorUsage(false, true)]
    private Color value;
}