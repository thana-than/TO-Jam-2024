using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Than.Input;

public class CharacterLook : MonoBehaviour
{
    public Brain brain;
    public Transform body;

    void Update()
    {
        Vector2 lookInput = brain.Look;
        if (lookInput == Vector2.zero)
            lookInput = brain.Move;

        if (lookInput == Vector2.zero)
            return;

        Vector3 rotation = body.eulerAngles;
        rotation.y = -Mathf.Atan2(lookInput.y, lookInput.x) * Mathf.Rad2Deg + 90;
        body.eulerAngles = rotation;
    }
}
