using UnityEngine;
using System.Collections;
using Fusion;

public class SpeedLow : NetworkBehaviour
{
    [Header("Low Settings")]
    public float LowMultiplier = 0.8f;
    public float LowDuration = 2.5f;
    public float LowForce = 4000f;
    public float LowDrag = 2f;

    [Header("Camera Settings")]
    public float LowFOVIncrease = -6f;
    public float fovSmoothSpeed = 4f;

    [Networked] private bool Used { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (Used) return;
        if (!Object.HasStateAuthority) return; // sadece host karar versin

        CarController car = other.GetComponentInParent<CarController>();
        if (car == null) return;

        Rigidbody rb = car.GetComponent<Rigidbody>();
        if (rb == null) return;

        Used = true;

        NetworkObject carNo2 = car.GetComponent<NetworkObject>();
        if (carNo2 != null && carNo2.InputAuthority != PlayerRef.None)
        {
            RPC_StartBoostFOV(carNo2.InputAuthority, LowDuration);
        }

        StartCoroutine(Boost(car, rb));
    }

    private IEnumerator Boost(CarController car, Rigidbody rb)
    {
        float originalMotorForce = car.GetMotorForce();
        float originalDrag = rb.drag;

        NetworkObject carNo = car.GetComponent<NetworkObject>();
        bool doFov = carNo != null && carNo.HasInputAuthority;

        Camera cam = doFov ? car.vehicleCamera : null;
        float originalFOV = cam != null ? cam.fieldOfView : 0f;
        float targetFOV = originalFOV + LowFOVIncrease;

        car.SetMotorForce(originalMotorForce * LowMultiplier);
        rb.drag = LowDrag;

        float timer = 0f;
        while (timer < LowDuration)
        {
            rb.AddForce(-car.transform.forward * LowForce * Time.fixedDeltaTime, ForceMode.Force);

            if (cam != null)
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovSmoothSpeed);

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        car.SetMotorForce(originalMotorForce);
        rb.drag = originalDrag;

        if (cam != null)
            StartCoroutine(ResetFOV(cam, originalFOV));

        if (Object != null && Object.HasStateAuthority && Runner != null)
            Runner.Despawn(Object);
        else
            Destroy(gameObject);
    }

    private IEnumerator ResetFOV(Camera cam, float originalFOV)
    {
        while (Mathf.Abs(cam.fieldOfView - originalFOV) > 0.1f)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, originalFOV, Time.deltaTime * fovSmoothSpeed);
            yield return null;
        }
        cam.fieldOfView = originalFOV;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_StartBoostFOV([RpcTarget] PlayerRef targetPlayer, float duration)
    {
        CarController localCar = FindLocalCar();
        if (localCar == null || localCar.vehicleCamera == null) return;

        StartCoroutine(BoostFOVOnly(localCar.vehicleCamera, duration));
    }

    private CarController FindLocalCar()
    {
        foreach (var car in FindObjectsOfType<CarController>())
        {
            var no = car.GetComponent<NetworkObject>();
            if (no != null && no.HasInputAuthority)
                return car;
        }
        return null;
    }

    private IEnumerator BoostFOVOnly(Camera cam, float duration)
    {
        float originalFOV = cam.fieldOfView;
        float targetFOV = originalFOV + LowFOVIncrease;

        float t = 0f;
        while (t < duration)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovSmoothSpeed);
            t += Time.deltaTime;
            yield return null;
        }

        while (Mathf.Abs(cam.fieldOfView - originalFOV) > 0.1f)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, originalFOV, Time.deltaTime * fovSmoothSpeed);
            yield return null;
        }

        cam.fieldOfView = originalFOV;
    }
}