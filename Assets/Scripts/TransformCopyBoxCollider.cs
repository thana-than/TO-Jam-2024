using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TransformCopyBoxCollider : MonoBehaviour
{
    public BoxCollider2D boxCollider;

    void Update()
    {
        if (!boxCollider) return;
        transform.localPosition = boxCollider.offset;
        transform.localScale = new Vector3(boxCollider.size.x, boxCollider.size.y, 1);
    }
}
