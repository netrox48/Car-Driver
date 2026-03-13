using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private bool isBraking;

    private float currentSteerAngle;
    private float currentBrakeForce;

    [Header("Car Settings")]
    public float motorForce = 1500f;
    public float brakeForce = 3000f;
    public float maxSteerAngle = 30f;

    [Header("Wheel Colliders")]
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    [Header("Wheel Transforms")]
    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;

    [Header("Respawn")]
    public float holdTimeForRespawn = 3f;

    private Rigidbody rb;
    private bool useNetworkBridge = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetUseNetworkBridge(bool value)
    {
        useNetworkBridge = value;
    }

    void FixedUpdate()
    {
        if (useNetworkBridge) return;

        GetInput();
        TickMotorSteerWheels();
    }

    void GetInput()
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

    void HandleMotor()
    {
        rearLeftWheelCollider.motorTorque = verticalInput * motorForce;
        rearRightWheelCollider.motorTorque = verticalInput * motorForce;

        currentBrakeForce = isBraking ? brakeForce : 0f;
        ApplyBraking();
    }

    void ApplyBraking()
    {
        frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        rearLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearRightWheelCollider.brakeTorque = currentBrakeForce;
    }

    void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;

        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    void UpdateSingleWheel(WheelCollider wc, Transform wt)
    {
        if (wc == null || wt == null) return;

        wc.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wt.position = pos;
        wt.rotation = rot;
    }

    public Rigidbody GetRigidbody()
    {
        return rb;
    }

    public float GetMotorForce()
    {
        return motorForce;
    }

    public void SetMotorForce(float value)
    {
        motorForce = value;
    }
}