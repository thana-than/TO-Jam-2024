using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GameObjectByString Variable", menuName = "Variables/GameObject/FindByString")]
public class GameObjectVariable_ByString : GameObjectVariable
{
    [HideInInspector] public override GameObject Value => GetTrackedObject();

    public string objectString;
    public ObjectStringType stringType;
    public enum ObjectStringType { name, tag }
    GameObject trackedObject;

    GameObject GetTrackedObject()
    {
        if (!trackedObject)
        {
            switch (stringType)
            {
                case ObjectStringType.name:
                    trackedObject = GameObject.Find(objectString);
                    break;
                case ObjectStringType.tag:
                    trackedObject = GameObject.FindGameObjectWithTag(objectString);
                    break;
            }
        }

        return trackedObject;
    }
}
