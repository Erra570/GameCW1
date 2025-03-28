using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThirdViewMvt : MonoBehaviour
{
    // Required variables
    public Animator animator;
    public Rigidbody myRb;
    public Transform myThirdViewCam;

    // Variables for the player force factor
    public float movementForce = 10f;
    public float jumpForce = 5f;

    // Variables for the player's movement
    public float standingSpeed;
    public float runningSpeed;


    // Variables for the player's crouching state
    public float crouchingSpeed;
    public float crouchingHeight;

    // Variables for the player's falling state
    public float fallingSpeedMultiply;
    public float toSmoothMvt_Time;
    private float _turnSmoothVelocity;

    // Booleans to go from a state to another (crouching, rewinding, running, pushing)
    private float _standingHeight;
    private bool _isCrouching = false;
    private bool _isRewindOn = false;
    private bool _isRunning = false;
    private bool _isPushing = false;

    // Variables for pushing objects
    private GameObject _pushableObject;
    public float pushForce = 3f;
    public float maxPushDistance = 2f;

    // Variables for the game over panel
    private Vector3 firstPosition;
    public float maxFallDistance = -20f;
    public GameObject gameOverPanel;
    private Vector3 lastGroundedPosition;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        myRb = GetComponent<Rigidbody>();
        myRb.freezeRotation = true;
        firstPosition = transform.position;
        //Debug.Log("Standing height : " + _standingHeight);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    // Update is called once per frame
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

                if (moveInput < 0)
                {
                    StopPushing();
                    return;
                }

                float distance = Vector3.Distance(transform.position, _pushableObject.transform.position);
                if (distance > maxPushDistance)
                {
                    Debug.Log("Stopped pushing: too far from the object.");
                    StopPushing();
                    return;
                }

                Rigidbody rb = _pushableObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 moveDir = transform.forward * moveInput * standingSpeed * Time.deltaTime;
                    rb.MovePosition(rb.position + moveDir);
                    myRb.MovePosition(rb.position + moveDir);
                    myRb.AddForce(moveDir);
                }
            }

            // Falling :
            if (Physics.Raycast(transform.position, Vector3.down, 1.1f))
            {
                lastGroundedPosition = transform.position;
            }
            if (lastGroundedPosition.y - transform.position.y > maxFallDistance)
            {
                GameOver();
            }

            // Moving :
            if (!_isPushing)
            {
                if (direction.magnitude >= 0.1f)
                {
                    // Rotate the player towards movement direction
                    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + myThirdViewCam.eulerAngles.y;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, toSmoothMvt_Time);
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);

                    // Move in the intended direction
                    Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                    myRb.velocity = new Vector3(moveDir.x * (_isCrouching ? crouchingSpeed : (_isRunning ? runningSpeed : standingSpeed)),
                    myRb.velocity.y, // Keep vertical velocity (jumping, falling)
                    moveDir.z * (_isCrouching ? crouchingSpeed : (_isRunning ? runningSpeed : standingSpeed)));

                    animator.SetBool("isWalking", true);
                }
                else
            {
                animator.SetBool("isWalking", false);
                // Stop horizontal movement when no input
                myRb.velocity = new Vector3(0, myRb.velocity.y, 0);
            }
            }   
            else
            {
                float moveInput = Input.GetAxisRaw("Vertical");
                if (moveInput != 0f && _pushableObject != null)
                {
                    Rigidbody rb = _pushableObject.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 moveDir = transform.forward * moveInput * pushForce * Time.deltaTime;
                        rb.MovePosition(rb.position + moveDir);
                        myRb.velocity = new Vector3(moveDir.x, myRb.velocity.y, moveDir.z);
                    }
                    animator.SetBool("isWalking", true);
                }
                else
                {
                    animator.SetBool("isWalking", false);
                    myRb.velocity = new Vector3(0, myRb.velocity.y, 0);
                }
            }
            // Crouching : 
            if (Input.GetButtonDown("Crouch") && !_isRunning)
            {
                _isCrouching = !_isCrouching;
                animator.SetBool("isCrouching", _isCrouching);

                CapsuleCollider col = GetComponent<CapsuleCollider>();
                col.height = _isCrouching ? crouchingHeight : _standingHeight;
                col.center = new Vector3(col.center.x, col.height / 2, col.center.z); // Adjust center to avoid sinking into the ground


                // to change once we have a real character :
                // transform.localScale = new Vector3(1, _isCrouching?crouchingHeight:_standingHeight, 1); //The "shape"
                //myController.Move(Vector3.down * 0.1f);
            }
            // Running :
            if (Input.GetButton("Run") && !_isCrouching)
            {
                _isRunning = true;
                animator.SetBool("isRunning", true);
            } else if (Input.GetButtonUp("Run")){
                _isRunning = false;
                animator.SetBool("isRunning", false);
            }


            // Pushing :
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

            // Restarting the game after a game over
            if (gameOverPanel.activeSelf)
            {
                Time.timeScale = 0;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    RestartGame();
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

    // Game over function
    void GameOver()
    {
        Debug.Log("Dead!");
        gameOverPanel.SetActive(true);
    }

    // Restarting the game
    void RestartGame()
    {
        gameOverPanel.SetActive(false);
        Time.timeScale = 1;
        transform.position = firstPosition;
    }

    // Toggling the rewind mode
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
        Rewindable[] rewindableObjects = GameObject.FindObjectsOfType<Rewindable>();
        foreach (Rewindable obj in rewindableObjects)
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
        LayerMask rewindableLayer = LayerMask.GetMask("Rewindable");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, rewindableLayer))
        {
            if (hit.collider.GetComponent<Rewindable>() != null)
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

    // Pushing objects
    void StartPushing()
    {
        if (_isPushing || _pushableObject != null) return;
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 2f))
        {
            if (hit.collider.GetComponent<Pushable>() != null)
            {
                RecordAndRewind rewindScript = hit.collider.GetComponent<RecordAndRewind>();
                animator.SetBool("isPushing", true);
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
                    rb.constraints = RigidbodyConstraints.FreezeRotation;
                    FixedJoint newJoint = _pushableObject.AddComponent<FixedJoint>();
                    newJoint.connectedBody = GetComponent<Rigidbody>();
                    newJoint.breakForce = 1000;
                }
                Debug.Log("you can now push yipee!!");

            }
        }
        else
        {
            Debug.Log("There is nothing to push");
        }
    }

    // Stopping pushing objects
    void StopPushing()
    {
        if (_pushableObject != null)
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

            animator.SetBool("isPushing", false);
            Debug.Log("you stopped pushing");
        }
    }
}

