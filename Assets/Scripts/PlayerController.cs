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
    public AudioSource dashSFX;
    public AudioSource deathSFX;

    [Header("Debugging")]
    public GameObject hitPoint;
    public Movable selectedMovable;

    bool cableShot;

    float timer = 0f;
    float timeToContact;

    float cableDistance;


    bool cableEnabled = false;
    bool isGrounded;
    bool canJump = true;

    bool airBoostEnabled = true;

    bool isPulling = false;
    bool isReleasing = false;

    Ray groundRay;

    bool undeteattachable = false;
    bool hasPressed;

    Vector2 direction;
    Vector2 rotateDirection;

    [HideInInspector] public bool checkVerticalSpeed = true;


    public static PlayerController local;

    float yRotation;
    // Start is called before the first frame update

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void Start()
    {

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

        controls.Gameplay.Restart.performed += RestartGame;


        GameManager.RestartEvent += OnRestart;
        GameManager.NextLevelEvent += OnNextLevel;

        Cursor.lockState = CursorLockMode.Locked;
        jumpSFX.clip = data.jumpSFX;
        shotSFX.clip = data.shotSFX;
        pullSFX.clip = data.pullSFX;
        landSFX.clip = data.landSFX;
        dashSFX.clip = data.airBoostSFX;
        deathSFX.clip = data.deathSFX;

        hitPoint.transform.parent = null;
        hitPoint.SetActive(false);

        line.transform.parent = null;
        line.useWorldSpace = true;
        line.enabled = false;

        

        local = this;

        if (data.levelFailed)
        {
            deathSFX.Play();
            data.levelFailed = false;
        }

        if (data.checkPoint)
        {
            if (CheckPoint.local)
            {
                transform.position = CheckPoint.local.transform.position;
                transform.rotation = CheckPoint.local.transform.rotation;
            }
        }

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
                airBoostEnabled = true;

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

        if (!isGrounded)
        {           

            Vector2 horizontalSpeed = new Vector2(rb.velocity.x, rb.velocity.z);

            if (horizontalSpeed.magnitude > data.maxAirHorizontalSpeed)
            {
                horizontalSpeed = Vector2.ClampMagnitude(horizontalSpeed, data.maxAirHorizontalSpeed);

                rb.velocity = new Vector3(Mathf.Lerp(rb.velocity.x, horizontalSpeed.x, data.velocityDamper), rb.velocity.y, Mathf.Lerp(rb.velocity.z, horizontalSpeed.y, data.velocityDamper));
            }

            if (checkVerticalSpeed)
            {
                if (Mathf.Abs(rb.velocity.y) > data.maxAirVerticalSpeed)
                {
                    rb.velocity = new Vector3(rb.velocity.x, Mathf.Lerp(rb.velocity.y, Mathf.Sign(rb.velocity.y) * data.maxAirVerticalSpeed, data.velocityDamper), rb.velocity.z);
                }
            }


        }
        else
        {
 
            

            Vector2 horizontalSpeed = new Vector2(rb.velocity.x, rb.velocity.z);

            if (horizontalSpeed.magnitude > data.maxSpeed)
            {
                horizontalSpeed = Vector2.ClampMagnitude(horizontalSpeed, data.maxSpeed);

                rb.velocity = new Vector3(horizontalSpeed.x, rb.velocity.y, horizontalSpeed.y);
            }
        }


        if (hasPressed)
        {
            StartShoot();
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


                UIController.UIColorChangeEvent?.Invoke(Color.cyan);
            }
            else
            {

                UIController.UIColorChangeEvent?.Invoke(Color.white);
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


    public void RotateMask(InputAction.CallbackContext context)
    {
        rotateDirection = context.ReadValue<Vector2>();
    }

    public void StopRotating(InputAction.CallbackContext context)
    {
        rotateDirection = Vector2.zero;
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

        if (selectedMovable)
        {
            selectedMovable.Move(transform.position, true) ;
            //RemoveCable();

        }
        else
        {
            if (cableEnabled)
            {
                isPulling = true;
                isReleasing = false;


                pullSFX.Play();
            }
            else
            {
                AirBoost();
            }
        }





    }
    public void StopPulling(InputAction.CallbackContext context)
    {
        isPulling = false;
    }


    public void ReleaseEvent(InputAction.CallbackContext context)
    {
        if (selectedMovable)
        {
            selectedMovable.Move(transform.position, false);
            //RemoveCable();
        }
        else
        {

            if (cableEnabled)
            {
                isReleasing = true;
                isPulling = false;
            }
        }

    }

    public void StopReleaseEvent(InputAction.CallbackContext context)
    {
        isReleasing = false;
    }


    public void AirBoost()
    {
        if (isGrounded)
        {
            return;
        }

        if (!airBoostEnabled)
        {
            return;
        }

        rb.velocity = Vector3.zero;
        rb.AddForce(view.transform.forward * data.airBoostForce, ForceMode.VelocityChange);
        airBoostEnabled = false;
        dashSFX.Play();
    }



    public void ShootEvent(InputAction.CallbackContext context)
    {
        if (cableShot)
        {
            return;
        }

        if (undeteattachable)
        {
            return;
        }


        if (!cableEnabled)
        {
            hasPressed = true;
            CancelInvoke("EndPress");
            Invoke("EndPress", data.maxCableActivationDelay);
        }
        else
        {
            if (!isGrounded)
            {
                rb.AddForce(view.transform.forward * data.cableReleaseBoost, ForceMode.VelocityChange);
            }
            
            RemoveCable();
        }

    }

    public void RemoveCable()
    {
        line.enabled = false;
        cableEnabled = false;
        hook.transform.localRotation = Quaternion.identity;
        cableDistance = 9999f;
        hasPressed = false;
        selectedMovable = null;
        hitPoint.SetActive(false);
    }

    public void EndPress()
    {
        hasPressed = false;
    }

    public void StartShoot()
    {
        RaycastHit hitInfo;

        Ray ray = new Ray(view.transform.position, view.transform.forward);

        if (Physics.Raycast(ray, out hitInfo, data.maxCableLength, data.attachableLayer))
        {
            shotSFX.Play();
            cableShot = true;
            hitPoint.SetActive(true);
            hitPoint.transform.parent = null;
            hitPoint.transform.position = hitInfo.point;
            timeToContact = Vector3.Distance(hook.attachPoint.position, hitPoint.transform.position) / data.cableLaunchSpeed;
            line.enabled = true;
            line.SetPositions(new Vector3[2] { hook.attachPoint.position, hook.attachPoint.position });
            line.material = hook.unattachMaterial;
            cableEnabled = true;
            UIController.UIColorChangeEvent?.Invoke(Color.white);
            
            hasPressed = false;
            undeteattachable = true;
            CancelInvoke("AllowDeattach"    );
            Invoke("AllowDeattach", data.undeattachableTimer);
            airBoostEnabled = true;

            Movable movable = hitInfo.collider.GetComponentInParent<Movable>();

            if (movable)
            {
                selectedMovable = movable;
                hitPoint.transform.parent = selectedMovable.transform;

            }
            else 
            { 
                selectedMovable = null;
            }


        }

    }

    public void AllowDeattach()
    {
        undeteattachable = false;
    }

    public void AdvanceCable()
    {
        timer += Time.deltaTime;

        if(timer >= timeToContact)
        {
            timer = 0f; 
            
            cableDistance = Vector3.Distance(transform.position, hitPoint.transform.position);
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
        float distanceToAttachPoint = Vector3.Distance(transform.position, hitPoint.transform.position);


        if(distanceToAttachPoint > cableDistance)
        {


            Vector3 attachDirection = hitPoint.transform.position - transform.position;
            
           

           rb.AddForce(attachDirection.normalized * data.cableTension*(distanceToAttachPoint-cableDistance)*Time.deltaTime, ForceMode.Acceleration);
            
        }
    }

    public void UnClampVerticalSpeed()
    {
        checkVerticalSpeed = false;
        CancelInvoke("ClampVerticalSpeed");
        Invoke("ClampVerticalSpeed", 1f);

    }

    public void ClampVerticalSpeed()
    {
        checkVerticalSpeed = true;
    }

    public void CanJump()
    {
        canJump = true;
    }

    public void UpdateCable()
    {
        line.SetPositions(new Vector3[2] { hook.attachPoint.position, hitPoint.transform.position });
        hook.transform.LookAt(hitPoint.transform.position);

        if (isReleasing)
        {
            if(cableDistance < data.maxCableLength) 
            {
                cableDistance += data.cableReleaseSpeed * Time.deltaTime;
            }
            
        }

        if (isPulling)
        {
            if(cableDistance > data.minCableLenght)
            cableDistance -= data.cablePullSpeed * Time.deltaTime;
        }


    }


    public void RestartGame(InputAction.CallbackContext context)
    {
        GameManager.RestartGame();
    }


    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    public void OnRestart()
    {
        GameManager.RestartEvent -= OnRestart;
        GameManager.NextLevelEvent -= OnNextLevel;
        controls.Gameplay.Jump.performed -= Jump;

        controls.Gameplay.Move.performed -= MoveMask;
        controls.Gameplay.Move.canceled -= StopMoving;

        controls.Gameplay.Rotate.performed -= RotateMask;
        controls.Gameplay.Rotate.canceled -= StopRotating;

        controls.Gameplay.Fire.performed -= ShootEvent;

        controls.Gameplay.Pull.performed -= PullEvent;
        controls.Gameplay.Pull.canceled -= StopPulling;

        controls.Gameplay.Release.performed -= ReleaseEvent;
        controls.Gameplay.Release.canceled -= StopReleaseEvent;

        controls.Gameplay.Restart.performed -= RestartGame;
    }

    public void OnNextLevel()
    {
        data.checkPoint = false;
        OnRestart();
    }

}
