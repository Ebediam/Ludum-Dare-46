using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public PlayerControls controls;
    [Header("Data")]
    public PlayerData data;
    public Camera view;

    [Header("Physics")]
    public Rigidbody rb;
    public Transform groundCheck;

    [Header("Hook")]
    public Hook hook;
    public LineRenderer line;

    [Header("Sound effects")]
    public AudioSource jumpSFX;
    public AudioSource shotSFX;
    public AudioSource pullSFX;
    public AudioSource landSFX;

    [Header("Debugging")]
    public GameObject hitPoint;

    bool cableShot;

    float timer = 0f;
    float timeToContact;

    float maxCableDistance;

    bool cableEnabled = false;
    bool isGrounded;
    bool canJump = true;

    bool isPulling = false;
    bool isReleasing = false;

    Ray groundRay;

    bool canAttach;

    Vector2 direction;
    Vector2 rotateDirection;
    

   

    float yRotation;
    // Start is called before the first frame update
    void Awake()
    {
        controls = new PlayerControls();
        controls.Gameplay.Jump.performed += Jump;

        controls.Gameplay.Move.performed += MoveMask;
        controls.Gameplay.Move.canceled += StopMoving;

        controls.Gameplay.Rotate.performed += RotateMask;
        controls.Gameplay.Rotate.canceled += StopRotating;

        controls.Gameplay.Fire.performed += ShootEvent;

        controls.Gameplay.Pull.performed += PullEvent;
        controls.Gameplay.Pull.canceled += StopPulling;

        controls.Gameplay.Release.performed += ReleaseEvent;
        controls.Gameplay.Release.canceled += StopReleaseEvent;

    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        jumpSFX.clip = data.jumpSFX;
        shotSFX.clip = data.shotSFX;
        pullSFX.clip = data.pullSFX;
        landSFX.clip = data.landSFX;

        hitPoint.transform.parent = null;
        line.transform.parent = null;
        line.useWorldSpace = true;


    }

    public void RotateMask(InputAction.CallbackContext context)
    {
        rotateDirection = context.ReadValue<Vector2>();
    }

    public void StopRotating(InputAction.CallbackContext context)
    {
        rotateDirection = Vector2.zero;
    }

    void Update()
    {
        if (canJump)
        {
            groundRay = new Ray(groundCheck.transform.position, transform.up * -1f);
            Debug.DrawRay(groundCheck.transform.position, transform.up * 0.3f * -1f, Color.red);
            if (Physics.Raycast(groundRay, out RaycastHit hitInfo, 0.3f, data.groundLayer))
            {
                if (!isGrounded)
                {
                    landSFX.Play();
                }

                isGrounded = true;

            }
            else
            {
                
                isGrounded = false;
            }
        }

        if (direction != Vector2.zero)
        {
            Move(direction);
        }

        if (rotateDirection.x != 0)
        {
            Rotate(rotateDirection);
        }

        if (rotateDirection.y != 0)
        {
            Look(rotateDirection.y);
        }

        if (cableShot)
        {
            AdvanceCable();
        }
        else
        {
            if (cableEnabled)
            {
                UpdateCable();
                AtractPlayer();

            }
        }

        if (cableEnabled)
        {           

            /*Vector2 horizontalSpeed = new Vector2(rb.velocity.x, rb.velocity.z);

            if (horizontalSpeed.magnitude > data.maxAirHorizontalSpeed)
            {
                horizontalSpeed = Vector2.ClampMagnitude(horizontalSpeed, data.maxAirHorizontalSpeed);

                rb.velocity = new Vector3(Mathf.Lerp(rb.velocity.x, horizontalSpeed.x, data.velocityDamper), rb.velocity.y, Mathf.Lerp(rb.velocity.z, horizontalSpeed.y, data.velocityDamper));
            }

            if (Mathf.Abs(rb.velocity.y) > data.maxAirVerticalSpeed)
            {
                rb.velocity = new Vector3(rb.velocity.x, Mathf.Lerp(rb.velocity.y, Mathf.Sign(rb.velocity.y)*data.maxAirVerticalSpeed, data.velocityDamper), rb.velocity.z);
            }*/
        }
        else
        {
            Vector2 horizontalSpeed = new Vector2(rb.velocity.x, rb.velocity.z);

            if (horizontalSpeed.magnitude > (data.maxSpeed + data.movementBoost))
            {
                horizontalSpeed = Vector2.ClampMagnitude(horizontalSpeed, (data.maxSpeed + data.movementBoost));

                rb.velocity = new Vector3(horizontalSpeed.x, rb.velocity.y, horizontalSpeed.y);
            }
        }

        

    }

    private void FixedUpdate()
    {
        if (!cableEnabled)
        {

            RaycastHit hitInfo;

            Ray ray = new Ray(view.transform.position, view.transform.forward);

            if (Physics.Raycast(ray, out hitInfo, data.maxCableLength, data.attachableLayer))
            {

                canAttach = true;
                UIController.pointer.color = Color.cyan;
            }
            else
            {
                canAttach = false;
                UIController.pointer.color = Color.white;
            }
        }
    }


    public void MoveMask(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }

    public void StopMoving(InputAction.CallbackContext context)
    {
        direction = Vector2.zero;
    }

    public void Move(Vector2 direction2D)
    {
        Vector3 direction = new Vector3(direction2D.x, 0f, direction2D.y);

        if (isGrounded)
        {
            direction *= data.acceleration;
        }
        else
        {
            direction *= data.airAcceleration;
        }

        

        rb.AddRelativeForce(direction * Time.deltaTime, ForceMode.Acceleration);

    }


    public void Rotate(Vector2 direction2D)
    {

        if (Mathf.Abs(direction2D.x) > 0.3f)
        {
            transform.Rotate(transform.up, data.xMouseSensitivity * direction2D.x);
        }


    }

    public void Look(float angle)
    {


        yRotation += angle * data.yMouseSensitivity;
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);

        view.transform.localRotation = Quaternion.Euler(-yRotation, 0, 0);
    }


    public void PullEvent(InputAction.CallbackContext context) { 
    
        if(cableEnabled)
        {
            isPulling = true;
            isReleasing = false;
                     

            pullSFX.Play();
        }


    }

    public void StopPulling(InputAction.CallbackContext context)
    {
        isPulling = false;
    }

    public void ShootEvent(InputAction.CallbackContext context)
    {
        if (!cableEnabled)
        { 

            RaycastHit hitInfo;

            Ray ray = new Ray(view.transform.position, view.transform.forward);

            if (Physics.Raycast(ray, out hitInfo, data.maxCableLength, data.attachableLayer))
            {
                shotSFX.Play();
                cableShot = true;
                hitPoint.transform.position = hitInfo.point;
                timeToContact = Vector3.Distance(hook.attachPoint.position, hitPoint.transform.position) / data.cableLaunchSpeed;
                line.enabled = true;
                line.material = hook.unattachMaterial;
                cableEnabled = true;
                UIController.pointer.color = Color.white;

            }



        }
        else
        {
            line.enabled = false;
            cableEnabled = false;
            hook.transform.localRotation = Quaternion.identity;
            maxCableDistance = 9999f;
        }

        

    }


    public void AdvanceCable()
    {
        timer += Time.deltaTime;

        if(timer >= timeToContact)
        {
            timer = 0f; 
            
            maxCableDistance = Vector3.Distance(hook.attachPoint.position, hitPoint.transform.position);
            cableShot = false;
            line.material = hook.attachMaterial;
        }
        else
        {
            Vector3 cablePosition = Vector3.Lerp(hook.attachPoint.position, hitPoint.transform.position, timer / timeToContact);
            line.SetPositions(new Vector3[2] { hook.attachPoint.position, cablePosition });
            hook.transform.LookAt(cablePosition);

        }
    }

    public void Jump(InputAction.CallbackContext context)
    {

        if (isGrounded)
        {
            if (canJump)
            {
                rb.AddForce(transform.up * data.jumpForce, ForceMode.VelocityChange);
                isGrounded = false;
                jumpSFX.Play();
                canJump = false;
                Invoke("CanJump", 0.1f);

            }
        }


    }

    public void AtractPlayer()
    {
        float distanceToAttachPoint = Vector3.Distance(hook.attachPoint.position, hitPoint.transform.position);

        if(distanceToAttachPoint > maxCableDistance)
        {
            Vector3 attachDirection = hitPoint.transform.position - hook.attachPoint.position;
            


            rb.AddForce(attachDirection * data.cableTension*(distanceToAttachPoint-maxCableDistance), ForceMode.Acceleration);
            
        }
    }

    public void ReleaseEvent(InputAction.CallbackContext context)
    {
        if (cableEnabled)
        {
            isReleasing = true;
            isPulling = false;
        }
    }

    public void StopReleaseEvent(InputAction.CallbackContext context)
    {
        isReleasing = false;
    }

    public void CanJump()
    {
        canJump = true;
    }

    public void UpdateCable()
    {
        line.SetPosition(0, hook.attachPoint.position);
        hook.transform.LookAt(hitPoint.transform.position);

        if (isReleasing)
        {
            maxCableDistance += data.cableReleaseSpeed * Time.deltaTime;
        }

        if (isPulling)
        {
            maxCableDistance -= data.cablePullSpeed * Time.deltaTime;
        }


    }



    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }


}
