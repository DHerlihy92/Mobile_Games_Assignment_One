using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonManager : MonoBehaviour
{
    private Gyroscope gyro = Input.gyro;
    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // loads current scene
    }

    public void SwitchGyro()
    {
        if(gyro.enabled)
        {
            gyro.enabled = false;
        }
        else
        gyro.enabled = true;
    }
}
