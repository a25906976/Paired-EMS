using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllController : MonoBehaviour
{
    public AirController airController;
    public EMSController emsController;
    public float delayTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            StartCoroutine("emsFirst");
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            //airController.arduino.ArduinoWrite("a " + airController.air_duration + "\n");
            StartCoroutine("airFirst");
            //emsController.arduinoEMS.ArduinoWrite("e " + emsController.onTime + "\n");
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            emsController.arduinoEMS.ArduinoWrite("e " + emsController.onTime + "\n");
            airController.arduino.ArduinoWrite("a " + airController.air_duration + "\n");
        }

    }

    IEnumerator emsFirst()
    {
        emsController.arduinoEMS.ArduinoWrite("e " + emsController.onTime + "\n");
        yield return new WaitForSeconds(delayTime);
        airController.arduino.ArduinoWrite("a " + airController.air_duration + "\n");
    }

    IEnumerator airFirst()
    {
        airController.arduino.ArduinoWrite("a " + airController.air_duration + "\n");
        yield return new WaitForSeconds(delayTime);
        emsController.arduinoEMS.ArduinoWrite("e " + emsController.onTime + "\n");
    }

}
