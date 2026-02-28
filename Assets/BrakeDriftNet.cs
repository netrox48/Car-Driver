using Fusion;
using UnityEngine;

public class BrakeDriftNet : NetworkBehaviour
{
    [Header("Refs")]
    public Rigidbody rb;
    public Transform forwardRef;

    [Header("When Space is held")]
    public float minSpeedToDrift = 8f;

    [Range(0f, 1f)] public float sideGripNormal = 0.90f;
    [Range(0f, 1f)] public float sideGripDrift = 0.45f;

    [Header("Feel")]
    public float extraYawTorque = 10f;
    public float sideSlipAssist = 25f;
    public float recoverStrength = 8f;

    void Reset()
    {
        rb = GetComponent<Rigidbody>();
        forwardRef = transform;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        if (rb == null || forwardRef == null) return;

        if (!GetInput(out CarInputData input)) return;

        bool handbrake = input.Brake;
        float speed = rb.velocity.magnitude;

        bool drifting = handbrake && speed > minSpeedToDrift;

        Vector3 v = rb.velocity;
        Vector3 fwd = forwardRef.forward;
        Vector3 right = forwardRef.right;

        float vFwd = Vector3.Dot(v, fwd);
        float vSide = Vector3.Dot(v, right);

        float grip = drifting ? sideGripDrift : sideGripNormal;

        float targetSide = vSide * grip;

        float lerpSpeed = drifting ? 2.5f : recoverStrength;

        Vector3 targetV = fwd * vFwd + right * targetSide;
        rb.velocity = Vector3.Lerp(v, targetV, Runner.DeltaTime * lerpSpeed);

        if (drifting)
        {
            rb.AddTorque(Vector3.up * input.Horizontal * extraYawTorque, ForceMode.Acceleration);

            float sign = Mathf.Sign(vSide == 0 ? input.Horizontal : vSide);
            rb.AddForce(right * sign * sideSlipAssist, ForceMode.Acceleration);
        }
    }
}