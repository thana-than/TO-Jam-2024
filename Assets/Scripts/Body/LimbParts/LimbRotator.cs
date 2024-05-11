using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Than.Input;

public class LimbRotator : MonoBehaviour
{
    Brain brain;

    float rotation = 0;
    float rotationLock = 45;

    void Start()
    {
        brain = GetComponentInParent<Brain>();
    }

    void Update()
    {
        if (brain.Move.value == Vector2.zero)
            return;

        rotation = Mathf.Clamp(brain.Move.value.ToDeg(), -rotationLock, rotationLock);
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotation));
    }
}
