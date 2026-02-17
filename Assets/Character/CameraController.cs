using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Hedef Ayarlarý")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 2f, -4f);

    [Header("Mouse Ayarlarý")]
    public float mouseSensitivity = 100f;

    public bool isLocal = true;

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        if (!isLocal) return;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (!isLocal) return;
        if (target == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -30f, 60f);

        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        transform.rotation = rotation;

        transform.position = target.position + (rotation * offset);
    }
}
