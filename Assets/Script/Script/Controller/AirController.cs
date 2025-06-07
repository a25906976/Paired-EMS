using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirController : MonoBehaviour
{
    public ArduinoBasic arduino;

    public string force;
    public string air_duration;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            arduino.ArduinoWrite("a " + air_duration + "\n");
            //Debug.Log("GO");
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            SetForce();
        }
    }
    private void OnDestroy()
    {
        arduino.OnApplicationQuit();
    }

    void SetForce()
    {
        arduino.ArduinoWrite("f " + force + "\n");
    }
}
