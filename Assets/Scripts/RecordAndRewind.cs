using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordAndRewind : MonoBehaviour
{
    public float recordTime = 5f;
    private List<Vector3> _positions = new List<Vector3>();
    private List<Quaternion> _rotations = new List<Quaternion>();
    private bool _isRewinding = false;
    private bool _isBeingPushed = false;

    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // FixedUpdate is called once per unity-frame (0.02s by default)
    // unity-frames are not dependent on the framerate of the game so better to use for physics-related stuff!
    void FixedUpdate()
    {
        if (_isRewinding)
        {
            Rewind();
        }
        else
        {
            Record();
        }
    }

    private bool IsMoving()
    {
        return rb.velocity.magnitude > 0.01f || _positions.Count == 0 || transform.position != _positions[_positions.Count - 1];
    }

    private void Record()
    {
        if (!IsMoving()) return; // If the object is not moving, we don't record its position
        if (_positions.Count > Mathf.Round(recordTime / Time.fixedDeltaTime))
        {
            _positions.RemoveAt(0);
            _rotations.RemoveAt(0);
        }
        _positions.Add(transform.position);
        _rotations.Add(transform.rotation);
    }

    public void Rewind()
    {
        if (_positions.Count > 1)
        {
            Vector3 lastPos = _positions[_positions.Count - 1];
            Quaternion lastRot = _rotations[_rotations.Count - 1];

            // Calculate velocity based on previous position (smooth rewind effect)
            Vector3 rewindVelocity = (lastPos - transform.position) / Time.fixedDeltaTime;

            rb.velocity = rewindVelocity;  // Apply velocity instead of teleporting
            rb.MoveRotation(lastRot);      // Preserve rotation with physics

            // Remove the last position after applying it
            _positions.RemoveAt(_positions.Count - 1);
            _rotations.RemoveAt(_rotations.Count - 1);
        }
        else
        {
            StopRewind();
        }
    }


    public void StartRewind()
    {
        if (_isBeingPushed) return;

        _isRewinding = true;
        rb.isKinematic = false; // Keep physics enabled
        rb.velocity = Vector3.zero; // Prevent sudden movements
        rb.mass = 1000000;
        rb.angularVelocity = Vector3.zero;
        Debug.Log("Rewinding...");
    }

    public void StopRewind()
    {
        _isRewinding = false;
    
        // Keep current velocity to avoid instant stop
        rb.mass = 5;
        rb.velocity *= 0.5f; // Reduce it gradually instead of stopping suddenly
        rb.angularVelocity = Vector3.zero;

        Debug.Log("Rewind stopped.");
    }


    public void SetBeingPushed(bool isBeingPushed){
        _isBeingPushed = isBeingPushed;
    }

    public bool IsBeingPushed(){
        return _isBeingPushed;
    }
    
    public bool IsRewinding(){
        return _isRewinding;
    }   

}