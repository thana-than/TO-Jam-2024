using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class MovingPath_Point : MonoBehaviour
{
    MovingPath path;
    void OnValidate()
    {
        Refresh();
    }
    void OnDestroy()
    {
        Refresh();
    }

    void Refresh()
    {
#if UNITY_EDITOR
        if (gameObject.scene.name == null)
            return;
#endif
        if (!path)
            path = GetComponentInParent<MovingPath>();

        path.Refresh();
    }
}
