using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Joystick joystick;
    public float speed = 2.5f;
    public float maxVelocityChange = 5f;
    public float tiltAmount = 10f;

    private Vector3 velocityVector = Vector3.zero;  //inital veclocity
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();   
    }

    void Update()
    {
        //Taking the joystick inputs
        float _xMovementInput = joystick.Horizontal;
        float _zMovementInput = joystick.Vertical;

        //Calculating velocity vectors
        Vector3 _movementHorizontal = transform.right * _xMovementInput;
        Vector3 _movementVertical = transform.forward * _zMovementInput;

        //Calculate the final movement velocity vector
        Vector3 _movementVelocityVector = (_movementHorizontal + _movementVertical).normalized * speed;

        //Apply movement
        Move(_movementVelocityVector);
        
        //Applying tilt to spinning tops
        transform.rotation = Quaternion.Euler(joystick.Vertical * speed * tiltAmount, 0f, -1 * joystick.Horizontal * speed * tiltAmount);

    }

    void FixedUpdate()
    {
        if(velocityVector != Vector3.zero)
        {
            //Get rigibody's current velocity
            Vector3 velocity = rb.velocity;

            //Amount of velocity we should add to reach the velocity coming from input
            Vector3 velocityChange = (velocityVector - velocity);

            //Apply force to spinner top to reach the target velocity i.e. final movement velocity
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);//Limiting x comp. change in velocity to stop indefinite acceleration(velocity forever increasing)
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);//Limiting z comp. change in velocity to stop indefinite acceleration(velocity forever increasing)
            velocityChange.y = 0f;

            rb.AddForce(velocityChange, ForceMode.Acceleration);
        }


    }

    void Move(Vector3 movementVelocityVector)
    {
        velocityVector = movementVelocityVector;
    }


}
