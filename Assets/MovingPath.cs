using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MovingPath : MonoBehaviour
{
    public Transform followingTransform;
    public Transform points_root;
    Transform[] points = new Transform[0];

    public float speed;
    [SerializeField][Range(0, 1)] float percent;

    int len;
    float max_distance = 0;
    float[] distances = new float[0];
    float time = 0;


    void OnValidate()
    {
        Refresh();
    }

    public async void Refresh()
    {
#if UNITY_EDITOR
        if (gameObject.scene.name == null)
            return;
#endif
        await Task.Delay(1);
        RefreshArray();
        UpdatePosition();
    }

    void Update()
    {
        time += Time.deltaTime * speed;
        percent = Mathf.PingPong(time, 1);
        UpdatePosition();
    }

    void UpdatePosition()
    {
        if (len > 1)
            followingTransform.position = Evaluate(percent);
    }

    Vector2 Evaluate(float percent)
    {
        float distance = max_distance * percent;

        int index = 1;

        while (index < len && distances[index] < distance)
            index++;

        float t = Mathf.InverseLerp(distances[index - 1], distances[index], distance);
        return Vector2.Lerp(points[index - 1].position, points[index].position, t);
    }

    void RefreshArray()
    {
        points = points_root.GetChildren();
        len = points.Length;
        max_distance = 0;
        distances = new float[len];
        distances[0] = 0;
        for (int i = 1; i < len; i++)
        {
            float dist = Vector2.Distance(points[i].transform.position, points[i - 1].transform.position);
            max_distance += dist;
            distances[i] = max_distance;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        for (int i = 1; i < len; i++)
        {
            if (!points[i - 1] || !points[i])
                return;
            Gizmos.DrawLine(points[i - 1].position, points[i].position);
        }
    }
}