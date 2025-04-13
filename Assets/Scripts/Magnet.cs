using UnityEngine;

public class Magnet : MonoBehaviour
{
    private GameObject attachedObject;
    private FixedJoint joint;

    private void OnTriggerEnter(Collider other) //simple trigger to detect pickable objects
    {
        if (attachedObject == null && other.CompareTag("Pickup"))
        {
            Rigidbody targetRb = other.attachedRigidbody;
            if (targetRb != null)
            {
                joint = gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = targetRb;
                joint.breakForce = 500f;
                joint.breakTorque = 500f;

                attachedObject = other.gameObject;
                Debug.Log("Picked up: " + attachedObject.name);
            }
        }
    }

    private void Update()
    {
        if (attachedObject != null && Input.GetKeyDown(KeyCode.J))
        {
            DropObject();
        }
    }

    void DropObject()
    {
        if (joint != null)
        {
            Destroy(joint);
        }
        attachedObject = null;
        Debug.Log("dropped object.");
    }
}
