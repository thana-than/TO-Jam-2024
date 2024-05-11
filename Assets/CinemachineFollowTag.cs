using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CinemachineFollowTag : MonoBehaviour
{
    public string followTag = "Player";
    CinemachineVirtualCamera cam;
    void Awake()
    {
        Follow();
    }

    // Update is called once per frame
    void OnValidate()
    {
        Follow();
    }

    void Follow()
    {
        if (!cam)
            cam = GetComponent<CinemachineVirtualCamera>();

        GameObject tagged = GameObject.FindGameObjectWithTag(followTag);
        cam.Follow = tagged.transform;
    }
}
