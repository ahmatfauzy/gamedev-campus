using UnityEngine;
using UnityEngine.EventSystems;

public class mobil : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private bool useGasAndBrake = true;

    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking;
    private Rigidbody rb;

    private float uiGasInput;
    private float uiMundurInput;
    private bool uiRemInput;

    [Header("Settings")]
    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float antiRollForce = 5000f;

    [Header("Center of Mass")]
    [SerializeField] private Vector3 centerOfMass = new Vector3(0, -0.5f, 0);

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [Header("Wheel Meshes")]
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass;

        HubungkanTombol("Gas", TekanGas, LepasGas);
        HubungkanTombol("Rem", TekanRem, LepasRem);
        HubungkanTombol("Mundur", TekanMundur, LepasMundur);
    }

    private void HubungkanTombol(string nama, UnityEngine.Events.UnityAction down, UnityEngine.Events.UnityAction up)
    {
        GameObject tombol = GameObject.Find(nama);
        if (tombol != null)
        {
            EventTrigger trigger = tombol.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = tombol.AddComponent<EventTrigger>();

            EventTrigger.Entry entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            entryDown.callback.AddListener((data) => { down(); });
            trigger.triggers.Add(entryDown);

            EventTrigger.Entry entryUp = new EventTrigger.Entry();
            entryUp.eventID = EventTriggerType.PointerUp;
            entryUp.callback.AddListener((data) => { up(); });
            trigger.triggers.Add(entryUp);
        }
    }

    public void TekanGas() { uiGasInput = 1f; }
    public void LepasGas() { uiGasInput = 0f; }
    public void TekanRem() { uiRemInput = true; }
    public void LepasRem() { uiRemInput = false; }
    public void TekanMundur() { uiMundurInput = -1f; }
    public void LepasMundur() { uiMundurInput = 0f; }

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        AntiRoll();
    }

    private void GetInput()
    {
        if (useGasAndBrake)
        {
            horizontalInput = SimpleInput.GetAxis("Horizontal");
            verticalInput = 0f;
            isBreaking = false;

            bool gas = SimpleInput.GetButton("Gas") || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
            bool rem = SimpleInput.GetButton("Rem") || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
            bool mundur = SimpleInput.GetButton("Mundur") || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            if (rem || uiRemInput)
            {
                verticalInput = 0f;
                isBreaking = true;
            }
            else if (gas || uiGasInput > 0f)
            {
                verticalInput = 1f;
                isBreaking = false;
            }
            else if (mundur || uiMundurInput < 0f)
            {
                verticalInput = -1f;
                isBreaking = false;
            }
            else
            {
                verticalInput = 0f;
                isBreaking = false;
            }
        }
        else
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            isBreaking = Input.GetKey(KeyCode.Space);
        }
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;

        currentbreakForce = isBreaking ? breakForce : 0f;

        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
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
        Vector3 pos;
        Quaternion rot;

        wheelCollider.GetWorldPose(out pos, out rot);

        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void AntiRoll()
    {
        WheelHit hit;
        float travelL = 1f;
        float travelR = 1f;

        bool groundedL = frontLeftWheelCollider.GetGroundHit(out hit);
        if (groundedL)
            travelL = (-frontLeftWheelCollider.transform.InverseTransformPoint(hit.point).y - frontLeftWheelCollider.radius) / frontLeftWheelCollider.suspensionDistance;

        bool groundedR = frontRightWheelCollider.GetGroundHit(out hit);
        if (groundedR)
            travelR = (-frontRightWheelCollider.transform.InverseTransformPoint(hit.point).y - frontRightWheelCollider.radius) / frontRightWheelCollider.suspensionDistance;

        float force = (travelL - travelR) * antiRollForce;

        if (groundedL)
            rb.AddForceAtPosition(frontLeftWheelCollider.transform.up * -force, frontLeftWheelCollider.transform.position);
        if (groundedR)
            rb.AddForceAtPosition(frontRightWheelCollider.transform.up * force, frontRightWheelCollider.transform.position);

        bool groundedRearL = rearLeftWheelCollider.GetGroundHit(out hit);
        if (groundedRearL)
            travelL = (-rearLeftWheelCollider.transform.InverseTransformPoint(hit.point).y - rearLeftWheelCollider.radius) / rearLeftWheelCollider.suspensionDistance;

        bool groundedRearR = rearRightWheelCollider.GetGroundHit(out hit);
        if (groundedRearR)
            travelR = (-rearRightWheelCollider.transform.InverseTransformPoint(hit.point).y - rearRightWheelCollider.radius) / rearRightWheelCollider.suspensionDistance;

        force = (travelL - travelR) * antiRollForce;

        if (groundedRearL)
            rb.AddForceAtPosition(rearLeftWheelCollider.transform.up * -force, rearLeftWheelCollider.transform.position);
        if (groundedRearR)
            rb.AddForceAtPosition(rearRightWheelCollider.transform.up * force, rearRightWheelCollider.transform.position);
    }
}