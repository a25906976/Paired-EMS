using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMSController : MonoBehaviour
{

    // 
    public ArduinoBasic arduinoEMS;
    public ArduinoBasic arduinoAir;
    public string onTime;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //arduinoAir.ArduinoWrite("a 700 250");
            arduinoEMS.ArduinoWrite("e "+ onTime + "\n");
            //arduinoEMS.ArduinoWrite("t " + onTime + "\n");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            arduinoEMS.ArduinoWrite("f");
            StartCoroutine(timer(0.3f));
            arduinoEMS.ArduinoWrite("e " + onTime + "\n");
            StartCoroutine(timer(0.5f));
            arduinoEMS.ArduinoWrite("b");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            arduinoEMS.ArduinoWrite("t " + onTime + "\n");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            arduinoEMS.ArduinoWrite("f");
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            arduinoEMS.ArduinoWrite("o " + onTime + "\n");
        }
    }
    private void OnDestroy()
    {
        arduinoEMS.OnApplicationQuit();
        arduinoAir.OnApplicationQuit();
    }
    IEnumerator timer(float timeGap)
    {
        yield return new WaitForSeconds(timeGap);
    }
}
