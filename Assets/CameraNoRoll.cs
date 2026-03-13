using UnityEngine;

public class CameraNoRoll : MonoBehaviour
{
    void LateUpdate()
    {
        var e = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, e.y, 0f);
    }
}