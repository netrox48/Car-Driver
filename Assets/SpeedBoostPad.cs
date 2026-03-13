using UnityEngine;
using System.Collections;
using Fusion;

public class SpeedBoostPad : NetworkBehaviour
{
    [Header("Boost Settings")]
    public float boostMultiplier = 1.5f;
    public float boostDuration = 2.5f;
    public float boostForce = 15000f;
    public float boostedDrag = 0.03f;

    [Header("Camera Settings")]
    public float boostFOVIncrease = 15f;
    public float fovSmoothSpeed = 4f;

    [Networked] private bool Used { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (Used) return;
        if (!Object.HasStateAuthority) return;

        CarController car = other.GetComponentInParent<CarController>();
        if (car == null) return;

        Rigidbody rb = car.GetComponent<Rigidbody>();
        if (rb == null) return;

        Used = true;

        NetworkObject carNo = car.GetComponent<NetworkObject>();
        if (carNo != null && carNo.InputAuthority != PlayerRef.None)
        {
            RPC_StartBoostFOV(carNo.InputAuthority, boostDuration);
        }

        StartCoroutine(Boost(car, rb));
    }

    private IEnumerator Boost(CarController car, Rigidbody rb)
    {
        float originalMotorForce = car.GetMotorForce();
        float originalDrag = rb.drag;

        NetworkObject carNo = car.GetComponent<NetworkObject>();
        bool doFov = carNo != null && carNo.HasInputAuthority;

        Camera cam = doFov ? car.GetComponentInChildren<Camera>(true) : null;
        float originalFOV = cam != null ? cam.fieldOfView : 0f;
        float targetFOV = originalFOV + boostFOVIncrease;

        car.SetMotorForce(originalMotorForce * boostMultiplier);
        rb.drag = boostedDrag;

        float timer = 0f;
        while (timer < boostDuration)
        {
            rb.AddForce(car.transform.forward * boostForce * Time.fixedDeltaTime, ForceMode.Force);

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
        if (localCar == null) return;

        Camera cam = localCar.GetComponentInChildren<Camera>(true);
        if (cam == null) return;

        StartCoroutine(BoostFOVOnly(cam, duration));
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
        float targetFOV = originalFOV + boostFOVIncrease;

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