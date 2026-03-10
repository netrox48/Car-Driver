using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class CarNetworkBridge : NetworkBehaviour
{
    private CarController car;
    private float rHoldTimer;

    private void Awake()
    {
        car = GetComponent<CarController>();
    }

    public override void Spawned()
    {
        // Local input/respawn kapansın, network input kullanılacak
        if (car != null)
            car.SetUseNetworkBridge(true);

        rHoldTimer = 0f;
    }

    public override void FixedUpdateNetwork()
    {
        if (car == null) return;

        if (GetInput(out CarInputData input))
        {
            // inputları arabaya ver
            car.SetInput(input.Horizontal, input.Vertical, input.Brake);

            // motor + respawn sadece authority tarafında
            if (Object.HasStateAuthority)
            {
                car.TickMotorSteerWheels();

                // ✅ R basılı tut -> respawn
                if (input.RespawnHeld)
                {
                    rHoldTimer += Runner.DeltaTime;

                    if (rHoldTimer >= car.holdTimeForRespawn)
                    {
                        CheckpointManager.Instance?.Respawn(car);
                        rHoldTimer = 0f;
                    }
                }
                else
                {
                    rHoldTimer = 0f;
                }
            }
        }
    }
}