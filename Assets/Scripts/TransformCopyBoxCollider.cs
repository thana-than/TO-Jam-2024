using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TransformCopyBoxCollider : MonoBehaviour
{
    public BoxCollider2D boxCollider;
    public bool offset = true;
    public bool scale = true;

    void Update()
    {
        if (!boxCollider) return;
        if (offset)
            transform.localPosition = boxCollider.offset;
        if (scale)
            transform.localScale = new Vector3(boxCollider.size.x, boxCollider.size.y, 1);
    }
}
