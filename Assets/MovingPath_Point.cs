using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        if (!path)
            path = GetComponentInParent<MovingPath>();

        path.Refresh();
    }
}
