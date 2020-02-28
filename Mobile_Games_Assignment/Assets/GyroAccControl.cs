using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroAccControl : MonoBehaviour
{
    #region Gyro Control Varialbles
    private Gyroscope gyro;
    #endregion

    #region Accelerometer Control Variables
    private Rigidbody rb;
    private float speed = 5f;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        //Initializing Rigidbody object to enable accelerometer
        rb = GetComponent<Rigidbody>();

        //Initializing Gyro Controls
        gyro = Input.gyro;
        if(!gyro.enabled)
        {
            gyro.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Rotating object with Gyroscope
        gameObject.transform.rotation = gyro.attitude;

        //Moving object with Accelerometer
        Vector3 acc = Input.acceleration;
        rb.AddForce(acc.x * speed, 0, acc.y * speed);
    }
}
