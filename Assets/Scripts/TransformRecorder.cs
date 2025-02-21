using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformRecorder : MonoBehaviour
{
    public float recordTime = 5f;
    private List<Vector3> positions = new List<Vector3>();
    private List<Quaternion> rotations = new List<Quaternion>();
    private bool isRewinding = false;

    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // FixedUpdate is called once per unity-frame (0.02s by default)
    //  unity-frames are not dependent on the framerate of the game so betetr to use for physics-related stuff!
    void FixedUpdate()
    {
        if (isRewinding)
        {
            Rewind();
        }
        else
        {
            Record();
        }
    }

    private void Record()
    {
        if (positions.Count > Mathf.Round(recordTime / Time.fixedDeltaTime))
        {
            positions.RemoveAt(0);
            rotations.RemoveAt(0);
        }
        positions.Add(transform.position);
        rotations.Add(transform.rotation);
    }

    public void Rewind(){
        if (positions.Count > 0)
        {
            transform.position = positions[positions.Count - 1];
            transform.rotation = rotations[rotations.Count - 1];
            positions.RemoveAt(positions.Count - 1);
            rotations.RemoveAt(rotations.Count - 1);
        } else {
            StopRewind();
        }
    }

    public void StartRewind()
    {
        isRewinding = true;
        rb.isKinematic = true;
    }

    public void StopRewind()
    {
        isRewinding = false;
        rb.isKinematic = false;
    }

}