using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdViewMvt : MonoBehaviour
{
    public CharacterController myController;
    public Transform myThirdViewCam;

    public float standingSpeed;
    public float fallingSpeedMultiply;

    public float toSmoothMvt_Time;
    private float _turnSmoothVelocity;

    public float crouchingSpeed;
    public float crouchingHeight;
    private float _standingHeight;
    private bool _isCrouching = false;
    private bool _isRewindOn = false;

    private bool _isPushing = false;
    private GameObject _pushableObject;
    public float pushForce = 3f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
        _standingHeight = myController.height;
        Debug.Log("Standing height : " + _standingHeight);
    }
    void Update()
    {
        // Here put everything that cannot be done while choosing an object to rewind!!
        if (!_isRewindOn)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            // Moving while pushing : 
            if (_isPushing && _pushableObject != null)
            {
                float moveInput = Input.GetAxisRaw("Vertical");
                if (moveInput != 0f)
                {
                    Rigidbody rb = _pushableObject.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 moveDir = transform.forward * moveInput * (_isCrouching ? crouchingSpeed : standingSpeed) * Time.deltaTime;
                        rb.MovePosition(rb.position + moveDir);
                        myController.Move(moveDir);
                    }
                }
            }

            // Moving :
            if (!_isPushing){
                if (direction.magnitude >= 0.1f)
                {
                    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + myThirdViewCam.eulerAngles.y;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, toSmoothMvt_Time);
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);

                    Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                    myController.Move(moveDir.normalized * (_isCrouching ? crouchingSpeed : standingSpeed) * Time.deltaTime);
                }
            } else {
                float moveInput = Input.GetAxisRaw("Vertical");
                if (moveInput != 0f && _pushableObject != null)
                {
                    Rigidbody rb = _pushableObject.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 moveDir = transform.forward * moveInput * pushForce * Time.deltaTime;
                        rb.MovePosition(rb.position + moveDir);
                        myController.Move(moveDir);
                    }
                }
            }

            // Falling :
            if (!myController.isGrounded)
            {
                Vector3 mvt = Vector3.zero + Physics.gravity;
                myController.Move(mvt * fallingSpeedMultiply *Time.deltaTime);
            }

            // Crouching : 
            if (Input.GetButtonDown("Crouch"))
            {
                _isCrouching = !_isCrouching;
                myController.height = _isCrouching ? crouchingHeight : _standingHeight; // the controller mesh

                // to change once we have a real character :
                transform.localScale = new Vector3(1, _isCrouching?crouchingHeight:_standingHeight, 1); //The "shape"

                myController.Move(Vector3.down * 0.1f);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (_isPushing)
                {
                    StopPushing();
                }
                else
                {
                    StartPushing();
                }
            }
        }

        // Managing the rewindable objects highlighting
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleRewind();
        }

        // Selecting a rewindable object with the mouse button
        if (_isRewindOn && Input.GetMouseButtonDown(0))
        {
            SelectRewindableObject();
        }
    }

    void ToggleRewind()
    {
        _isRewindOn = !_isRewindOn;
        HighlightRewindableObjects(_isRewindOn);
        Cursor.lockState = _isRewindOn ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _isRewindOn;
        Debug.Log("Rewind is " + (_isRewindOn ? "on" : "off"));
    }

    // Highlighting rewindable objects
    void HighlightRewindableObjects(bool highlight)
    {
        GameObject[] rewindableObjects = GameObject.FindGameObjectsWithTag("Rewindable");
        foreach (GameObject obj in rewindableObjects)
        {
            Renderer objRenderer = obj.GetComponent<Renderer>();
            if (objRenderer != null)
            {
                objRenderer.material.color = highlight ? Color.yellow : Color.white;
            }
        }
    }

    // Selecting a rewindable object
    void SelectRewindableObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 20f))
        {
            if (hit.collider.CompareTag("Rewindable"))
            {
                RecordAndRewind rewindScript = hit.collider.GetComponent<RecordAndRewind>();
                if (rewindScript != null)
                {
                    rewindScript.StartRewind();
                    _isRewindOn = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    HighlightRewindableObjects(_isRewindOn);
                }
            }
        } 
        else 
        {
            _isRewindOn = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            HighlightRewindableObjects(_isRewindOn);
            Debug.Log("You didn't select any rewindable object");
        }
    }

    void StartPushing()
    {
        if (_isPushing || _pushableObject != null) return;
        Ray ray = new Ray(transform.position + Vector3.up * (myController.height/2), transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 2f, Color.red, 2f);

        if (Physics.Raycast(ray, out hit, 2f))
        {
            if (hit.collider.CompareTag("Pushable"))
            {
                RecordAndRewind rewindScript = hit.collider.GetComponent<RecordAndRewind>();
                if (rewindScript != null && rewindScript.IsRewinding())
                {
                    Debug.Log("Cannot push, object is rewinding!");
                    return;
                }
                _isPushing = true;
                _pushableObject = hit.collider.gameObject;
                transform.rotation = Quaternion.Euler(0f, myThirdViewCam.eulerAngles.y, 0f);
                Debug.Log("I'm pushing " + _pushableObject);

                Rigidbody rb = _pushableObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = false;
                    rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

                    FixedJoint joint = _pushableObject.GetComponent<FixedJoint>();
                    if (joint != null)
                    {
                        Destroy(joint);
                    }
                    FixedJoint newJoint = _pushableObject.AddComponent<FixedJoint>();
                    newJoint.connectedBody = GetComponent<Rigidbody>();
                    newJoint.breakForce = 1000;
                }
                GetComponent<Animator>().SetBool("IsPushing", true);
                Debug.Log("you can now push yipee!!");

            }
        } else {
            Debug.Log("There is nothing to push");
        }
    }

    void StopPushing()
    {
        if(_pushableObject != null)
        {
            Rigidbody rb = _pushableObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.constraints = RigidbodyConstraints.None;
            }

            FixedJoint joint = _pushableObject.GetComponent<FixedJoint>();
            if (joint != null)
            {
                Destroy(joint);
            }   
            _isPushing = false;
            _pushableObject = null;

            _turnSmoothVelocity = 0;

            GetComponent<Animator>().SetBool("IsPushing", false);
            Debug.Log("you stopped pushing");
        }
    }
}
