using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private Rigidbody2D body;
    [SerializeField]
    private float accelPower;
    [SerializeField]
    private float steerPower;

    private float steeringInput;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();   
    }

    // Update is called once per frame
    void Update()
    {
        steeringInput = Input.GetAxis("Horizontal");
    }
    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            body.AddForce(accelPower * transform.up, ForceMode2D.Force);
        }
        if (Input.GetKey(KeyCode.S))
        {
            body.AddForce(accelPower * -transform.up, ForceMode2D.Force);
        }
        body.MoveRotation(body.rotation + (-steeringInput * steerPower));
    }
}
