using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class NetworkCarBridge : NetworkBehaviour
{
    [SerializeField] private CarController car;

    private VehicleSeat seat;
    private Camera vehicleCamera;

    private float rHold = 0f;

    public override void Spawned()
    {
        if (car == null) car = GetComponent<CarController>();

        seat = GetComponent<VehicleSeat>();
        vehicleCamera = GetComponentInChildren<Camera>(true);

        if (car != null)
            car.SetUseNetworkBridge(true);

        // Kamera sadece local oyuncuda açýk
        if (vehicleCamera != null)
            vehicleCamera.enabled = Object.HasInputAuthority;

        rHold = 0f;
    }

    public override void FixedUpdateNetwork()
    {
        if (car == null) return;

        // Kamera sadece InputAuthority'de açýk kalsýn
        if (vehicleCamera != null)
            vehicleCamera.enabled = Object.HasInputAuthority;

        // Sürüþ ve fizik sadece StateAuthority'de çalýþmalý (Host/Server)
        if (!Object.HasStateAuthority) return;

        if (!GetInput(out CarInputData input))
            return;

        // (Ýstersen lock/seat kontrolü burada kalabilir; þimdilik kaldýrýyoruz)
        car.SetInput(input.Horizontal, input.Vertical, input.Brake);
        car.TickMotorSteerWheels();

        // Respawn (StateAuthority'de)
        if (input.RespawnHeld)
        {
            rHold += Runner.DeltaTime;
            if (rHold >= car.holdTimeForRespawn)
            {
                if (CheckpointManager.Instance != null)
                    CheckpointManager.Instance.Respawn(car);

                rHold = 0f;
            }
        }
        else
        {
            rHold = 0f;
        }
    }
}
