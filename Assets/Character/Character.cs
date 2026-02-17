using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Ayarlar")]
    public float moveSpeed = 5f;
    public float turnSpeed = 15f;

    [Header("Referanslar")]
    public Transform cam;

    public bool isLocal = true;

    private CharacterController controller;
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (!isLocal) return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!isLocal) return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        animator.SetFloat("InputX", x);
        animator.SetFloat("InputZ", z);

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        float targetAngle = cam.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        controller.Move(Vector3.down * 9.81f * Time.deltaTime);
    }
}
