using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentBrakeForce;
    private bool isBraking;

    [Header("Car Settings")]
    [SerializeField] private float motorForce = 1500f;
    [SerializeField] private float brakeForce = 3000f;
    [SerializeField] private float maxSteerAngle = 30f;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [Header("Wheel Transforms")]
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    [Header("Cameras")]
    public Camera playerCamera;
    public Camera vehicleCamera;

    [Header("Exit Settings")]
    [SerializeField] private float exitSpeedLimit = 1.5f;
    [SerializeField] private Transform leftExitPoint;

    private Rigidbody carRb;

    [Header("Player")]
    public GameObject player;
    public MonoBehaviour playerMovementScript;
    public MonoBehaviour cameraLookScript;
    public Collider playerCollider;
    public Transform driverSeat;
    public GameObject playerModel;

    [Header("Enter Settings")]
    public float enterDistance = 3f;

    private bool isPlayerInside = false;

    [Header("Respawn")]
    public float holdTimeForRespawn = 3f;
    private float rHoldTimer = 0f;

    [Header("Network")]
    [SerializeField] private bool useNetworkBridge = false;

    private void Start()
    {
        if (vehicleCamera != null)
            vehicleCamera.enabled = false;

        carRb = GetComponent<Rigidbody>();
    }

    public void SetUseNetworkBridge(bool value)
    {
        useNetworkBridge = value;
    }

    public bool GetUseNetworkBridge()
    {
        return useNetworkBridge;
    }

    private void Update()
    {
        if (!useNetworkBridge)
            HandleRespawnInput();

        if (useNetworkBridge) return;

        if (player == null) return;

        if (!isPlayerInside)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance <= enterDistance && Input.GetKeyDown(KeyCode.F))
                EnterCar();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.F))
                TryExitCar();
        }
    }

    private void HandleRespawnInput()
    {
        if (Input.GetKey(KeyCode.R))
        {
            rHoldTimer += Time.deltaTime;

            if (rHoldTimer >= holdTimeForRespawn)
            {
                if (CheckpointManager.Instance != null)
                    CheckpointManager.Instance.Respawn(this);

                rHoldTimer = 0f;
            }
        }
        else
        {
            rHoldTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (useNetworkBridge) return;
        if (!isPlayerInside) return;

        GetInput();
        TickMotorSteerWheels();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBraking = Input.GetKey(KeyCode.Space);
    }

    public void SetInput(float h, float v, bool brake)
    {
        horizontalInput = h;
        verticalInput = v;
        isBraking = brake;
    }

    public void TickMotorSteerWheels()
    {
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void HandleMotor()
    {
        rearLeftWheelCollider.motorTorque = verticalInput * motorForce;
        rearRightWheelCollider.motorTorque = verticalInput * motorForce;

        currentBrakeForce = isBraking ? brakeForce : 0f;
        ApplyBraking();
    }

    private void ApplyBraking()
    {
        frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        rearLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearRightWheelCollider.brakeTorque = currentBrakeForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        if (wheelCollider == null || wheelTransform == null) return;

        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    private void EnterCar()
    {
        isPlayerInside = true;

        if (playerMovementScript != null) playerMovementScript.enabled = false;
        if (cameraLookScript != null) cameraLookScript.enabled = false;
        if (playerCollider != null) playerCollider.enabled = false;
        if (playerModel != null) playerModel.SetActive(false);

        if (driverSeat != null)
        {
            player.transform.SetParent(driverSeat);
            player.transform.localPosition = Vector3.zero;
            player.transform.localRotation = Quaternion.identity;
        }

        if (playerCamera != null) playerCamera.enabled = false;
        if (vehicleCamera != null) vehicleCamera.enabled = true;
    }

    private void TryExitCar()
    {
        if (carRb != null && carRb.velocity.magnitude > exitSpeedLimit)
            return;

        ExitCar();
    }

    private void ExitCar()
    {
        isPlayerInside = false;

        player.transform.SetParent(null);
        if (leftExitPoint != null)
            player.transform.position = leftExitPoint.position;

        if (playerMovementScript != null) playerMovementScript.enabled = true;
        if (cameraLookScript != null) cameraLookScript.enabled = true;
        if (playerCollider != null) playerCollider.enabled = true;
        if (playerModel != null) playerModel.SetActive(true);

        if (vehicleCamera != null) vehicleCamera.enabled = false;
        if (playerCamera != null) playerCamera.enabled = true;
    }

    public float GetMotorForce()
    {
        return motorForce;
    }

    public void SetMotorForce(float value)
    {
        motorForce = value;
    }

    public Rigidbody GetRigidbody()
    {
        return carRb;
    }

    public void ForceEnterLocal(GameObject p)
    {
        player = p;
        if (player == null) return;

        isPlayerInside = true;

        if (playerMovementScript != null) playerMovementScript.enabled = false;
        if (cameraLookScript != null) cameraLookScript.enabled = false;
        if (playerCollider != null) playerCollider.enabled = false;
        if (playerModel != null) playerModel.SetActive(false);

        if (driverSeat != null)
        {
            player.transform.SetParent(driverSeat);
            player.transform.localPosition = Vector3.zero;
            player.transform.localRotation = Quaternion.identity;
        }

        if (playerCamera != null) playerCamera.enabled = false;
        if (vehicleCamera != null) vehicleCamera.enabled = true;
    }
}
