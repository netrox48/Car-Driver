using UnityEngine;
using Fusion;

[RequireComponent(typeof(Camera))]
public class CameraLocalOnly : NetworkBehaviour
{
    private Camera cam;
    private AudioListener listener;

    void Awake()
    {
        cam = GetComponent<Camera>();
        listener = GetComponent<AudioListener>();
    }

    public override void Spawned()
    {
        bool local = Object.HasInputAuthority;

        cam.enabled = local;

        if (listener != null)
            listener.enabled = local;
    }
}