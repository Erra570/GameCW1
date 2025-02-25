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

    public void Rewind(){
        if (_positions.Count > 0)
        {
            transform.position = _positions[_positions.Count - 1];
            transform.rotation = _rotations[_rotations.Count - 1];
            _positions.RemoveAt(_positions.Count - 1);
            _rotations.RemoveAt(_rotations.Count - 1);
        } else {
            StopRewind();
        }
    }

    public void StartRewind()
    {
        if(_isBeingPushed){
            return;
        }
        _isRewinding = true;
        rb.isKinematic = true;
        Debug.Log("Rewinding...");
    }

    public void StopRewind()
    {
        _isRewinding = false;
        rb.isKinematic = false;
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