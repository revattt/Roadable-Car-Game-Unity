using UnityEngine;

public class AircraftFollower : MonoBehaviour
{
    public Transform target;
    public float followDistance = 15f;
    public float moveSpeed = 20f;
    public float rotationSpeed = 2f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position - target.forward * followDistance;
        Vector3 direction = (targetPos - transform.position).normalized;

        // move toward target position
        rb.velocity = direction * moveSpeed;

        // rotate toward the main aircraft
        Quaternion desiredRot = Quaternion.LookRotation(target.position - transform.position);
        rb.rotation = Quaternion.Slerp(rb.rotation, desiredRot, rotationSpeed * Time.fixedDeltaTime);
    }
}
