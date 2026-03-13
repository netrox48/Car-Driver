using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class NetworkCarBridge : NetworkBehaviour
{
    private CarController car;
    private float rHold;

    void Awake()
    {
        car = GetComponent<CarController>();
    }

    public override void Spawned()
    {
        car.SetUseNetworkBridge(true);
        rHold = 0f;
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out CarInputData input)) return;

        car.SetInput(input.Horizontal, input.Vertical, input.Brake);

        if (!Object.HasStateAuthority) return;

        car.TickMotorSteerWheels();

        if (input.RespawnHeld)
        {
            rHold += Runner.DeltaTime;

            if (rHold >= car.holdTimeForRespawn)
            {
                CheckpointManager.Instance?.Respawn(car);
                rHold = 0;
            }
        }
        else
        {
            rHold = 0;
        }
    }
}