using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private Rigidbody playerRB;
    public WheelColliders colliders;
    public WheelMeshes wheelMeshes;

    public float gasInput;
    public float brakeInput;
    public float steeringInput;
    public float motorPower;
    public float brakePower;
    private float speed;
    public float flySpeed = 20f;
    public float flyRotationSpeed = 80f;
    bool isFlying;
    public TextMeshProUGUI speedText;
    public GameObject aiAircraftPrefab;
    private GameObject spawnedAI;


    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F))
        {
            isFlying = !isFlying;

            // Disable gravity during fly mode
            playerRB.useGravity = !isFlying;
            playerRB.velocity = Vector3.zero;
            playerRB.angularVelocity = Vector3.zero;

            // Disable wheel colliders when flying
            if (isFlying)
            {
                colliders.FRWheel.enabled = false;
                colliders.FLWheel.enabled = false;
                colliders.RRWheel.enabled = false;
                colliders.RLWheel.enabled = false;
            }
            else
            {
                colliders.FRWheel.enabled = true;
                colliders.FLWheel.enabled = true;
                colliders.RRWheel.enabled = true;
                colliders.RLWheel.enabled = true;
            }
        }

        if (isFlying)
        {
            HandleFlying();
        }
        else
        {
            speed = playerRB.velocity.magnitude;
            CheckInput();
            ApplyMotor();
            ApplySteering();
            ApplyBrake();
            ApplyWheelPositions();
        }

        if (Input.GetKeyDown(KeyCode.K) && spawnedAI == null)
        {
            SpawnAI();
        }


    }
    void FixedUpdate()
    {
        //Speed in kmh
        if (speedText != null)
        {
            float speedKPH = playerRB.velocity.magnitude * 3.6f;
            speedText.text = Mathf.RoundToInt(speedKPH) + " km/h";
        }


        if (isFlying)
        {
            HandleFlying();
            playerRB.drag = 1f;         // controls how it slows down
            playerRB.angularDrag = 40f;  // reduces quick rotation
        }
        else
        {
            playerRB.drag = 0.1f;
            playerRB.angularDrag = 0.05f;
        }
    }

    void CheckInput()
    {
        gasInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");

        float movingDirection = Vector3.Dot(transform.forward, playerRB.velocity);
        if (movingDirection < -0.5f && gasInput > 0)
        {
            brakeInput = Mathf.Abs(gasInput);
        }
        else if (movingDirection > 0.5f && gasInput < 0)
        {
            brakeInput = Mathf.Abs(gasInput);
        }
        else
        {
            brakeInput = 0;
        }
    }

    void ApplyMotor()
    {
        colliders.RRWheel.motorTorque = motorPower * gasInput;
        colliders.RLWheel.motorTorque = motorPower * gasInput;
    }

    void ApplyBrake()
    {
        colliders.FRWheel.brakeTorque = brakeInput * brakePower * 0.7f;
        colliders.FLWheel.brakeTorque = brakeInput * brakePower * 0.7f;
        colliders.RRWheel.brakeTorque = brakeInput * brakePower * 0.3f;
        colliders.RLWheel.brakeTorque = brakeInput * brakePower * 0.3f;
    }

    void ApplySteering()
    {
        float maxSteerAngle = 35f;
        float steeringAngle = steeringInput * maxSteerAngle;
        colliders.FRWheel.steerAngle = steeringAngle;
        colliders.FLWheel.steerAngle = steeringAngle;
    }

    void ApplyWheelPositions()
    {
        UpdateWheel(colliders.FRWheel, wheelMeshes.FRWheel);
        UpdateWheel(colliders.FLWheel, wheelMeshes.FLWheel);
        UpdateWheel(colliders.RRWheel, wheelMeshes.RRWheel);
        UpdateWheel(colliders.RLWheel, wheelMeshes.RLWheel);
    }

    void UpdateWheel(WheelCollider coll, MeshRenderer wheelMesh)
    {
        coll.GetWorldPose(out Vector3 position, out Quaternion rotation);
        wheelMesh.transform.position = position;
        wheelMesh.transform.rotation = rotation;
    }
    void HandleFlying()
    {
        float thrustInput = Input.GetAxis("Vertical"); //w or s
        float rollInput = 0f;
        float pitchInput = 0f;

        // roll with a/d
        if (Input.GetKey(KeyCode.A)) rollInput = 1f;
        else if (Input.GetKey(KeyCode.D)) rollInput = -1f;

        //pitch with q/e
        if (Input.GetKey(KeyCode.Q)) pitchInput = 1f;
        else if (Input.GetKey(KeyCode.E)) pitchInput = -1f;

        // forward and backward thrust w and s
        Vector3 forwardForce = transform.forward * thrustInput * flySpeed;
        playerRB.AddForce(forwardForce, ForceMode.Acceleration);

        // Apply torque: pitch (x), roll (z)
        Vector3 torque = new Vector3(
            pitchInput * flyRotationSpeed,  // Pitch
            0f,                             // No yaw for now
            rollInput * flyRotationSpeed   // Roll
        );
        //roll correction when no input pressed for roll, it automatically centers, not working as of now
        playerRB.AddTorque(transform.TransformDirection(torque), ForceMode.Acceleration);
        if (Mathf.Abs(rollInput) < 0.01f)
        {
            Vector3 localAngularVelocity = transform.InverseTransformDirection(playerRB.angularVelocity);
            float rollCorrection = -localAngularVelocity.z * 2f;
            playerRB.AddTorque(transform.forward * rollCorrection, ForceMode.Acceleration);
        }
    }
    void SpawnAI()
    {
        if (aiAircraftPrefab == null || spawnedAI != null) return;

        Vector3 spawnPos = transform.position + transform.right * 10f;
        spawnedAI = Instantiate(aiAircraftPrefab, spawnPos, Quaternion.identity);

        AircraftFollower aiScript = spawnedAI.GetComponent<AircraftFollower>();
        if (aiScript != null)
        {
            aiScript.target = this.transform;
        }

        Debug.Log("AI aircraft deployed.");
    }

}

[System.Serializable]
public class WheelColliders
{
    public WheelCollider FRWheel;
    public WheelCollider FLWheel;
    public WheelCollider RRWheel;
    public WheelCollider RLWheel;
}

[System.Serializable]
public class WheelMeshes
{
    public MeshRenderer FRWheel;
    public MeshRenderer FLWheel;
    public MeshRenderer RRWheel;
    public MeshRenderer RLWheel;
}
