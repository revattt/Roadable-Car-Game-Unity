using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeDeployer : MonoBehaviour
{
    public GameObject ropeSegmentPrefab; //rope segments joined using hinge joint
    public GameObject magnetPrefab;
    public int segmentCount = 10;
    public float segmentLength = 0.3f;
    private List<GameObject> segments = new List<GameObject>();
    private bool ropeDeployed = false;
    public Transform ropeStartPoint;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (!ropeDeployed)
            {
                DeployRope();
                ropeDeployed = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            if (ropeDeployed)
            {
                DestroyRope();
                ropeDeployed = false;
            }
        }
    }
    void DeployRope()
    {
        Transform lastAnchor = ropeStartPoint;

        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 pos = lastAnchor.position - new Vector3(0, segmentLength, 0);
            GameObject newSeg = Instantiate(ropeSegmentPrefab, pos, Quaternion.identity, transform);

            Rigidbody rb = newSeg.GetComponent<Rigidbody>();
            HingeJoint joint = newSeg.GetComponent<HingeJoint>();

            //connect to last body
            joint.connectedBody = lastAnchor.GetComponent<Rigidbody>();

            segments.Add(newSeg);
            lastAnchor = newSeg.transform;
        }

        //add magnet at the end
        Vector3 magnetPos = lastAnchor.position - new Vector3(0, segmentLength, 0);
        GameObject magnet = Instantiate(magnetPrefab, magnetPos, Quaternion.identity, transform);

        Rigidbody magnetRb = magnet.GetComponent<Rigidbody>();
        HingeJoint magnetJoint = magnet.GetComponent<HingeJoint>();
        magnetJoint.connectedBody = lastAnchor.GetComponent<Rigidbody>();

        segments.Add(magnet);
    }

    void DestroyRope() //destroys rope after pressing H
    {
        foreach (GameObject seg in segments)
        {
            Destroy(seg);
        }
        segments.Clear();
    }


}
