using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.InputSystem.Editor;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class LocalMouseProcessor : InputProcessor<Vector2>
{
#if UNITY_EDITOR
    static LocalMouseProcessor()
    {
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        InputSystem.RegisterProcessor<LocalMouseProcessor>();
    }

    public override Vector2 Process(Vector2 value, InputControl control)
    {
        if (!LocalMouseRoot.Instance)
            return Vector2.zero;

        return LocalMouseRoot.Instance.MouseToAxis(value);
    }
}

// No registration is necessary for an InputParameterEditor.
// The system will automatically find subclasses based on the
// <..> type parameter.
#if UNITY_EDITOR
public class LocalMouseProcessorEditor : InputParameterEditor<LocalMouseProcessor>
{
    private GUIContent m_label = new GUIContent("LocalMouse");

    // protected override void OnEnable()
    // {
    //     // Put initialization code here. Use 'target' to refer
    //     // to the instance of MyValueShiftProcessor that is being
    //     // edited.
    // }

    public override void OnGUI()
    {
        EditorGUILayout.LabelField(m_label);
        // Define your custom UI here using EditorGUILayout.
        // target.valueShift = EditorGUILayout.Slider(m_SliderLabel,
        //     target.valueShift, 0, 10);
    }
}
#endif
