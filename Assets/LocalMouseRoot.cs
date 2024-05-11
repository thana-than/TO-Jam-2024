using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalMouseRoot : SingletonBehaviour<LocalMouseRoot>
{
    public Vector2 position => transform.position;

    Camera cam;

    Vector2 lastMouse;

    public Vector2 MouseToAxis(Vector2 mouseScreenPos)
    {
        if (!cam)
            cam = Camera.main;

        Vector2 mouseWorld = cam.ScreenToWorldPoint(mouseScreenPos);
        lastMouse = position.DirectionTowards(mouseWorld);
        return lastMouse;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, lastMouse * 5);
    }
}
