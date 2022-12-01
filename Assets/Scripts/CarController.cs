using UnityEngine;
using System.Collections.Generic;

public class CarController : MonoBehaviour {
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float rayDistance;
    [SerializeField] private LayerMask groundMask;
    [Header("Wheels")]
        [SerializeField] private List<EdgeInfo> edgeInfos; 
        [SerializeField] private float maxMotorTorque;
        [SerializeField] private float maxSteeringAngle;
        [SerializeField] private float brakeTorque;
    [Header("Turbo")]
        [SerializeField] private float turboForce; 
        [SerializeField] private float turboDecreasePerSecond;
        [SerializeField] private ParticleSystem fireParticle;
        [SerializeField] private ParticleSystem smokeParticle;
    [Header("Jump")]
        [SerializeField] private float jumpForce;
        [SerializeField] private float secondJumpForce;
        [SerializeField] private int extraJumps = 1;
    [Header("General")]
        [SerializeField] private float xyRotationSpeed;
        [SerializeField] private float zRotationSpeed;

    private bool grounded = false;
    private int jumps;
    private float verticalInput;
    private float horizontalInput;
    private bool airRotation;
    private float motor = 0;
    private float steering = 0;
    private bool brake = false;
    private float turbo = 0;
    private bool onTurbo = false;
    float turboCharge = 0;
    private bool rotatingAfterJump = false;
    private Quaternion initialRotation = Quaternion.identity;
    



    void Update()
    {
        checkGround();

        if (!rotatingAfterJump)
        {
            verticalInput = Input.GetAxis("Vertical");
            horizontalInput = Input.GetAxis("Horizontal");
        }
        turbo = Input.GetAxis("Turbo");
        brake = Input.GetButton("Brake");
        if (Input.GetButtonDown("Jump") && (grounded || (!grounded && jumps > 0)))
            jump();
        airRotation = Input.GetButton("Rotate");
    }

    private void checkGround()
    {
        grounded = Physics.Raycast(transform.position, transform.up * - 1, rayDistance, groundMask);
        if (grounded)
        {
            jumps = extraJumps;
            rotatingAfterJump = false;
        }
    }

    private void jump()
    {
        if (grounded || (verticalInput == 0 && horizontalInput == 0))
            rb.AddForce(transform.up * jumpForce * 100, ForceMode.Impulse);
        else if (!grounded)
        {
            Vector3 dir = ((transform.forward * verticalInput + transform.right * horizontalInput).normalized + Vector3.up).normalized;
            rb.AddForce(dir * secondJumpForce * 100, ForceMode.Impulse);
            rotatingAfterJump = verticalInput != 0 || horizontalInput != 0;
            initialRotation = Quaternion.identity;
            jumps--;
        }
    }

    void FixedUpdate()
    {
        manageMovement();
        manageTurbo();
        manageBrake();
    }

    private void manageMovement()
    {
        if (grounded)
            groundedMovement();
        else if (!airRotation && !rotatingAfterJump)
            airMovement();
        else
            rotate();
        if (rotatingAfterJump)
            rotationControl();
    }

    private void groundedMovement()
    {
        motor = maxMotorTorque * verticalInput;
        steering = maxSteeringAngle * horizontalInput;
        foreach (EdgeInfo axleInfo in edgeInfos)
        {
            if (axleInfo.Steering)
            {
                axleInfo.LeftWheel.steerAngle = steering;
                axleInfo.RightWheel.steerAngle = steering;
            }
            if (axleInfo.Motor)
            {
                axleInfo.LeftWheel.motorTorque = motor;
                axleInfo.RightWheel.motorTorque = motor;
            }
            applyLocalPositionToVisuals(axleInfo.LeftWheel, axleInfo.LeftWheelTransform);
            applyLocalPositionToVisuals(axleInfo.RightWheel, axleInfo.RightWheelTransform);
        }
    }

    private void airMovement()
    {
        transform.Rotate(new Vector3(verticalInput * xyRotationSpeed * Time.fixedDeltaTime, horizontalInput * xyRotationSpeed * Time.fixedDeltaTime, 0));
    }

    private void rotate()
    {
        transform.Rotate(new Vector3(verticalInput * xyRotationSpeed * Time.fixedDeltaTime, 0, -horizontalInput * zRotationSpeed * Time.fixedDeltaTime));
    }
    
    private void rotationControl()
    {
        if (transform.rotation.x >= initialRotation.x + 360 || transform.rotation.x <= initialRotation.x - 360 || transform.rotation.z >= initialRotation.z + 360 || transform.rotation.z <= initialRotation.z - 360)
            rotatingAfterJump = false;
    }

    private void manageTurbo()
    {
        if (turbo != 0 && turboCharge > 0)
        {
            rb.AddForce(turboForce * turbo * transform.forward, ForceMode.Impulse);
            turboCharge -= turboDecreasePerSecond * Time.deltaTime;
            if (!onTurbo)
            {
                onTurbo = true;
                fireParticle.Play();
                smokeParticle.Play();
            }
        } else if (turbo == 0 || turboCharge <= 0)
        {
            onTurbo = false;
            fireParticle.Stop();
            smokeParticle.Stop();
        }
    }

    private void manageBrake()
    {
        if (brake)
            foreach (EdgeInfo axleInfo in edgeInfos)
            {
                axleInfo.LeftWheel.brakeTorque = brakeTorque;
                axleInfo.RightWheel.brakeTorque = brakeTorque;

                applyLocalPositionToVisuals(axleInfo.LeftWheel, axleInfo.LeftWheelTransform);
                applyLocalPositionToVisuals(axleInfo.RightWheel, axleInfo.RightWheelTransform);
            }
        else
            foreach (EdgeInfo axleInfo in edgeInfos)
            {
                axleInfo.LeftWheel.brakeTorque = 0;
                axleInfo.RightWheel.brakeTorque = 0;

                applyLocalPositionToVisuals(axleInfo.LeftWheel, axleInfo.LeftWheelTransform);
                applyLocalPositionToVisuals(axleInfo.RightWheel, axleInfo.RightWheelTransform);
            }
    }

    private void applyLocalPositionToVisuals(WheelCollider collider, Transform wheelTransform)
    {     
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
     
        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }

    public bool FullTurbo()
    {
        return turboCharge == 100;
    }
    public void AddTurbo(float amount)
    {
        turboCharge += amount;
        turboCharge = Mathf.Clamp(turboCharge, 0, 100);
    }

    void OnCollisionEnter(Collision col)
    {
        rotatingAfterJump = false;
    }

    void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, transform.up * - 1 * rayDistance);
    }
}
