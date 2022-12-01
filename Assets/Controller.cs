using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private Rigidbody2D body;

    private float accelPower = 7.5f;
    private float steerPower = .5f;
    private float boostForce = 1500;
    private float driftPower=.5f;
    private float maxSpeedOnRoad=10;
    private float maxSpeedOffRoad=5;
    private float maxSpeedDrift=15;
    private float maxSpeedBoost=75;

    private float steeringInput;
    private float horizontalInput;

    private float maxSpeed;
    private float steeringSpeed;
    private float accelInput;
    private float rotationAngle;
    private float velocityVSUp;
    private float driftBoostTimer;

    public bool canDrive = true;
    private bool offRoad;
    private bool isDrifting;
    private bool driftActivated;

    [SerializeField]
    private GameObject particles;
    public static Controller instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        particles.SetActive(false);
        body = GetComponent<Rigidbody2D>();   
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        accelInput = Input.GetAxis("Vertical");
        if(offRoad)
        { 
            //speedoffroad
        }
        else
        {
            maxSpeed = maxSpeedOnRoad;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isDrifting= true;
            maxSpeed = maxSpeedDrift;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isDrifting = false;
            if (offRoad)
            {
                maxSpeed = maxSpeedOffRoad;
            }
            else
            {
                maxSpeed = maxSpeedOnRoad;
            }
        }
    }
    private void FixedUpdate()
    {
        if(canDrive)
        {
            applySteering();
            applyEngineForce();
            killOrthogonalVelocity();
            if (velocityVSUp > maxSpeed)
            {
                body.AddForce(-transform.up * (accelPower * 2), ForceMode2D.Force);
            }
            if (isDrifting && !driftActivated && steeringInput != 0)//activate drift
            {
                particles.SetActive(true);
                driftPower = .9f;
                steerPower = 2.5f;
                driftActivated = true;
            }
            else if ((!isDrifting && driftActivated) || steeringInput == 0)//deactivate drift
            {
                particles.SetActive(false);
                driftPower = .5f;
                steerPower = 1.5f;
                driftActivated = false;
            }
            if (driftActivated)
            {
                if (driftBoostTimer > 1)
                {
                    particles.GetComponent<ParticleSystem>().startColor = Color.yellow;
                }
                else
                {
                    particles.GetComponent<ParticleSystem>().startColor = Color.gray;
                }
                driftBoostTimer += Time.deltaTime;
            }
            else
            {
                if (driftBoostTimer > 1)
                {
                    applySpeedBoost();
                }
                driftBoostTimer = 0;
            }
       
            
        }
        else
        {

            body.velocity = Vector2.zero;
        }
        /**
        if (Input.GetKey(KeyCode.W))
        {
            body.AddForce(accelPower * transform.up, ForceMode2D.Force);
        }
        if (Input.GetKey(KeyCode.S))
        {
            body.AddForce(accelPower * -transform.up, ForceMode2D.Force);
        }
        body.MoveRotation(body.rotation + (-steeringInput * steerPower));
        **/
    }

    private void applySpeedBoost()
    {
        body.AddForce(transform.up * 15, ForceMode2D.Impulse);
    }

    private void killOrthogonalVelocity()
    {
        Vector2 forwardVel = transform.up * Vector2.Dot(body.velocity, transform.up);
        Vector2 rightVel=transform.right*Vector2.Dot(body.velocity, transform.right);
        body.velocity = forwardVel+ rightVel*driftPower;
    }

    private void applySteering()
    {
        if (horizontalInput < 0 && steeringInput > 0 && ((accelInput <= 0 && velocityVSUp > 0) || (accelInput >= 0 && velocityVSUp < 0)))
        {
            steeringInput = Mathf.Lerp(steeringInput, -1, Time.fixedDeltaTime);
        }
        else if (horizontalInput > 0 && steeringInput < 0 && ((accelInput <= 0 && velocityVSUp > 0) || (accelInput >= 0 && velocityVSUp < 0)))
        {
            steeringInput=Mathf.Lerp(steeringInput,1,Time.fixedDeltaTime);
        }
        else
        {
            steeringInput = horizontalInput;
        }
        float minSpeedTurn = body.velocity.magnitude / 8;
        minSpeedTurn = Mathf.Clamp01(minSpeedTurn);
        if (accelInput <= 0 && velocityVSUp > 0 || accelInput >= 0 && velocityVSUp < 0)
        {
            steerPower = Mathf.Lerp(steerPower, 0, Time.fixedDeltaTime * 2);
        }
        else if (driftActivated)
        {
            steerPower = 2.5f;
        }
        else
        {
            steerPower = 1.5f;
        }
        if (velocityVSUp <= 0)
        {
            rotationAngle += steeringInput * steerPower * minSpeedTurn;
        }
        else
        {
            rotationAngle-=steeringInput*steerPower* minSpeedTurn;
        }
        body.rotation= rotationAngle;
    }
    private void applyEngineForce()
    {
        velocityVSUp=Vector2.Dot(transform.up,body.velocity);
        if (velocityVSUp >= maxSpeed && accelInput > 0) { return; }

        if (velocityVSUp <= -maxSpeed * .5f && accelInput < 0) { return; }
        else { body.drag = 0; }
        Vector2 engineForce = transform.up * accelInput * accelPower;
        body.AddForce(engineForce,ForceMode2D.Force);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boost"))
        {
            body.AddForce(boostForce * transform.up, ForceMode2D.Force);
        }
        if (collision.CompareTag("FinishLine"))
        {
            gameManager.instance.addLap();
        }
    }
}
