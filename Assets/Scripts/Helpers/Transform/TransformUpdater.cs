using UnityEngine;

public class TransformUpdater : MonoBehaviour
{
    public Vector3 translate;
    public Vector3 rotate;
    public Vector3 scale;

    void Update()
    {
        float delta = Time.deltaTime;
        transform.localPosition += translate * delta;
        transform.localEulerAngles += rotate * delta;
        transform.localScale += scale * delta;
    }
}
