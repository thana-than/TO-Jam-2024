using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Than.Input;

public class LimbRotator : MonoBehaviour
{
    Brain brain;

    float rotation = 0;
    public float offset = -90;
    public float rotationLock = 75;

    void Start()
    {
        brain = GetComponentInParent<Brain>();
    }

    void Update()
    {
        if (brain.Move.value == Vector2.zero)
            return;

        rotation = Mathf.Clamp(brain.Move.value.ToDeg(), transform.parent.rotation.z - rotationLock, transform.parent.rotation.z + rotationLock);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotation + offset));
    }
}
