using UnityEngine;
using Fusion;

public class CameraLocalOnly : MonoBehaviour
{
    public UnityEngine.Behaviour[] disableIfNotLocal;

    private void Start()
    {
        NetworkObject no = GetComponentInParent<NetworkObject>();
        if (no == null) return;

        if (!no.HasInputAuthority)
        {
            for (int i = 0; i < disableIfNotLocal.Length; i++)
                if (disableIfNotLocal[i] != null)
                    disableIfNotLocal[i].enabled = false;

            AudioListener al = GetComponent<AudioListener>();
            if (al != null) al.enabled = false;

            Camera cam = GetComponent<Camera>();
            if (cam != null) cam.enabled = false;
        }
    }
}
