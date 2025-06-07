using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = System.Random;
using UnityEngine.UI;
using System.ComponentModel.Design.Serialization;

public class ForceAmplify : MonoBehaviour
{

    public ArduinoBasic arduinoEMS;
    public ArduinoBasic arduinoAir;

    //cauculate Newton of AirJet.
    static float PWMtoNewton_m = 0.0009f;
    static float PWMtoNewton_k = -0.3019f;
    static public float Newton(int PWM)
    {
        return (PWM == 0) ? 0f : (float)PWM * PWMtoNewton_m + PWMtoNewton_k;
    }
    static public int PWM(float Newton)
    {
        return (Newton == 0) ? 0 : (int)((Newton - PWMtoNewton_k) / PWMtoNewton_m);
    }

    int forceNum = 3;
    int durationNum = 3;
    float[] airJetForce = { 1f, 2f, 3f };
    int[] duration = { 100, 200, 300 };
    bool[,] checkBox = new bool[3, 3];
    Random random = new System.Random();
    int airIndex;
    int duraIndex;
    float baseForce = 0f;
    float airForce = 0f;
    float airNewton = 0f;
    int time;
    int roundTime = 1;
    int reveralTime = 0;
    float changeUnit = 0.1f;
    float randomForce = 0f;
    bool secondSection = false;


    bool isRest = false;
    int restTime = 60;
    float passTime = 0;
    bool isOver = false;

    public GameObject increaseButton;
    public GameObject decreaseButton;
    public GameObject okButton;
    public GameObject nextButton;

    public Text TextBreak;
    public Text TextRound;
    public Text TextNewton;
    public Text TextDuration;


    // Start is called before the first frame update
    void Start()
    {
        for(var i = 0; i < forceNum; i++)
        {
            for(var j = 0; j < durationNum; j++)
            {
                checkBox[i, j] = false;
            }
        }
        airIndex = random.Next(0, forceNum);
        duraIndex = random.Next(0, durationNum);
        checkBox[airIndex, duraIndex] = true;
        //increaseButton.SetActive(false);
        TextNewton.text = Convert.ToString(airJetForce[airIndex]) + " N";
        //TextDuration.text = Convert.ToString(duration[duraIndex]) + " ms";
        TextRound.text = Convert.ToString(roundTime);
        airNewton = airJetForce[airIndex];
        time = duration[duraIndex];
        Debug.Log("Baseline: " + airNewton);
        Debug.Log("Duration: " + time);
    }

    // Update is called once per frame
    void Update()
    {
        if (isRest)
        {
            passTime += Time.deltaTime;
            if (passTime <= restTime )
            {
                TextBreak.text = Convert.ToString((restTime - passTime));
            }
            else
            {
                isRest = false;
                passTime = 0f;
                TextBreak.text = "";
            }
        }
        if (!isRest && Input.GetKeyDown(KeyCode.A) && !isOver)
        {
            StartCoroutine("TriggerAir");
        }
        if (!isRest && Input.GetKeyDown(KeyCode.D) && !isOver)
        {
            airForce = PWM(airNewton);
            StartCoroutine("TriggerBoth");
        }
        if (!isRest && Input.GetKeyDown(KeyCode.W) && !isOver)
        {
            IncreaseButton();
        }
        if (!isRest && Input.GetKeyDown(KeyCode.S) && !isOver)
        {
            DecreaseButton();
        }
        if (!isRest && Input.GetKeyDown(KeyCode.R) && roundTime < 26)
        {
            NextButton();
        }
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            isRest = false;
            passTime = 0f;
            TextBreak.text = "";
        }
        if(!isRest && Input.GetKeyDown(KeyCode.E) && !isOver)
        {
            arduinoEMS.ArduinoWrite("e " + time + "\n");
        }
        if(roundTime == 26)
        {
            arduinoAir.ArduinoWrite("f " + 0 + "\n");
            TextRound.text = "25";
            isOver = true;
            TextBreak.text = "Study Over!";
        }
    }

    public void AirJetButton()
    {
        StartCoroutine("TriggerAir");
    }

    public void TriggerButton()
    {
        /*int airForce = PWM(airJetForce[airIndex]);
        arduinoAir.ArduinoWrite("f " + airForce + "\n");
        int time = duration[duraIndex];
        arduinoEMS.ArduinoWrite("e " + time + "\n");
        arduinoAir.ArduinoWrite("a " + time + "\n");*/
        airForce = PWM(airNewton);
        StartCoroutine("TriggerBoth");
    }

    public void NextButton()
    {
        nextButton.SetActive(false);
        okButton.SetActive(true);
        Debug.Log("Estimated Force: " + airNewton);
        Debug.Log("-----------------------------");

        bool allTrue = AllTrue();

        while (checkBox[airIndex, duraIndex] == true && !allTrue)
        {
            airIndex = random.Next(0, forceNum);
            duraIndex = random.Next(0, durationNum);
        }

        TextRound.text = Convert.ToString(++roundTime);
        if (roundTime % 3 == 1 && roundTime!=10) isRest = true;
        checkBox[airIndex, duraIndex] = true;
        baseForce = airJetForce[airIndex];
        time = duration[duraIndex];
        //randomForce = random.Next(-100, 100)/500f;
        airNewton = baseForce + randomForce;
        TextNewton.text = Convert.ToString(airNewton) + " N";
        //TextDuration.text = Convert.ToString(time) + " ms";
        Debug.Log("Baseline: " + baseForce);
        Debug.Log("Duration: " + time);
        if (allTrue)
        {
            TextNewton.text = "Study Over!";
            TextBreak.text = "Study Over!";
        }
        //okButton.SetActive(true);
        //nextButton.SetActive(false);
        //increaseButton.SetActive(false);
        //changeUnit = 0.1f;
    }
    public bool AllTrue()
    {
        foreach (var b in checkBox)
        {
            if (b == false) return false;
        }
        return true;
    }
    public void IncreaseButton()
    {
        if(airNewton < 3f) airNewton = Mathf.Round((airNewton + changeUnit) * 1000f) / 1000f;
        TextNewton.text = Convert.ToString(airNewton) + " N";
    }
    public void DecreaseButton()
    {
        if(airNewton > 0f) airNewton = Mathf.Round((airNewton - changeUnit)*1000f) / 1000f;
        TextNewton.text = Convert.ToString(airNewton) + " N";
    }
    public void OKButton()
    {
        reveralTime++;
        changeUnit = 0.05f;
        //increaseButton.SetActive(true);
        //okButton.SetActive(false);
        // nextButton.SetActive(true);

        /*if (!increaseButton.active)
        {
            increaseButton.SetActive(true);
            decreaseButton.SetActive(false);
        }

        else
        {
            increaseButton.SetActive(false);
            decreaseButton.SetActive(true);
        }*/
        Debug.Log("Reversal" + reveralTime + ": " + airNewton);
        if (reveralTime >= 5)
        {
            okButton.SetActive(false);
            nextButton.SetActive(true);
            reveralTime = 0;
        }
       
    }

    IEnumerator TriggerBoth()
    {
        arduinoAir.ArduinoWrite("f " + airForce + "\n");
        yield return new WaitForSeconds(0.3f);

        arduinoAir.ArduinoWrite("a " + time + "\n");
        yield return new WaitForSeconds(0.023f);
        arduinoEMS.ArduinoWrite("e " + (time-0.04f) + "\n");
    }
    IEnumerator TriggerAir()
    {
        airForce = PWM(airJetForce[airIndex]);
        arduinoAir.ArduinoWrite("f " + airForce + "\n");
        yield return new WaitForSeconds(0.3f);
        //int time = duration[duraIndex];
        arduinoAir.ArduinoWrite("a " + time + "\n");
    }

}
