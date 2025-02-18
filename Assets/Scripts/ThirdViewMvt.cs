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


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _standingHeight = myController.height;
        Debug.Log("Standing height : " + _standingHeight);
    }
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Crouching : 
        if (Input.GetButtonDown("Crouch"))
        {
            _isCrouching = !_isCrouching;
            myController.height = _isCrouching ? crouchingHeight : _standingHeight; // the controller mesh

            // to change once we have a real character :
            transform.localScale = new Vector3(1, _isCrouching?crouchingHeight:_standingHeight, 1); //The "shape"

            myController.Move(Vector3.down * 0.1f);
        }

        // Moving :
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + myThirdViewCam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, toSmoothMvt_Time);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            myController.Move(moveDir.normalized * (_isCrouching ? crouchingSpeed : standingSpeed) * Time.deltaTime);
        }

        // Falling :
        if (!myController.isGrounded)
        {
            Vector3 mvt = Vector3.zero + Physics.gravity;
            myController.Move(mvt * fallingSpeedMultiply *Time.deltaTime);
        }

    }
}
