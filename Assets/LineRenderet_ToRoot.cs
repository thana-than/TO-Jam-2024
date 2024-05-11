using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class LineRenderet_ToRoot : MonoBehaviour
{
    LineRenderer lineRenderer;
    void Update()
    {
        Refresh();
    }

    void Refresh()
    {
        if (!lineRenderer)
            lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.SetPosition(0, -transform.localPosition);
    }
}
