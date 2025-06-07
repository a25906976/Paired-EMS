using System;
using System.Collections;
using UnityEngine;
using Bhaptics.Tact.Unity;
using Valve.VR;

public class redis_code_sample : MonoBehaviour {
    //CrossHaptic's Setting
    Subscriber subscriber;
    Subscriber subscriber2;
    string testChannelName = "nonsymmetrical_event";
    string testChannelName2 = "symmetrical_event";
    string url = "localhost";
    ushort port = 6379;


    public string onTime; // The duration of the EMS.
    float passTime; // timer.
    float gapTime; // The cooldown time(in ms) of the EMS.(i.e. Time between two EMS impulses.)
    string lastAmp;
    string[] timeMap = { "350", "500" }; // passTime's timeMap. Different haptic event will use different timeMap.


    public bool enableEMS = false; // trun on/off the EMS.
    public bool bothSide = false; // Decide to use "both side stimulation" or not.
    public bool punch;
    bool msgRcv = true; // (message receive) read the msg from CrossHaptic or not.
    bool msgRcvBoxing = true;


    public ArduinoBasic arduinoEMS;

    public enum gameType { boxing, ragnoRock}; // Which game is being used.
    public gameType game;
    public enum stimMode { bothSide, vibra, airJet};
    public stimMode mode;

    //bHaptic setting.
    public HapticSource bHaptic;
    public HapticSource handBhaptic;
    public HapticClip[] hapticClip; // bHaptic vibration pattern.
    public enum bHapticPattern {Bicep10I, Bicep20I, Bicep30I, Bicep40I, Bicep50I, Bicep60I, Bicep70I, Bicep80I, Bicep90I, Bicep100I, 
        B10T20, B10T40, B10T60, B10T80, B10T100,
        B20T20, B20T40, B20T60, B20T80, B20T100,
        B30T20, B30T40, B30T60, B30T80, B30T100,
        B40T20, B40T40, B40T60, B40T80, B40T100,
        B50T20, B50T40, B50T60, B50T80, B50T100,
        B60T20, B60T40, B60T60, B60T80, B60T100,
        B70T20, B70T40, B70T60, B70T80, B70T100,
        B80T20, B80T40, B80T60, B80T80, B80T100,
        B90T20, B90T40, B90T60, B90T80, B90T100,
        B100T20, B100T40, B100T60, B100T80, B100T100,
        T50I,T100I,Both50I,Both100I, hand200
    };
    public bHapticPattern vibrationPattern;

    //steamVR vive controller Input.
    public SteamVR_Action_Boolean touchPad;
    

    void Start() {
        subscriber = new Subscriber(url, port);
        subscriber.SubscribeTo(testChannelName);
        subscriber.msgQueue.OnMessage(msg => msgHandler(msg.Message));
        //subscriber2.SubscribeTo(testChannelName2);
        //subscriber2.msgQueue.OnMessage(msg => msgHandler2(msg.Message));
    }


    // Update is called once per frame
    void Update() {
        // get proper haptic clip.
        int index = Array.IndexOf(Enum.GetValues(vibrationPattern.GetType()), vibrationPattern);
        GetComponent<HapticSource>().clip = hapticClip[index];

        // get proper game's gapTime.
        switch (game) {
            case gameType.boxing:
                gapTime = float.Parse(timeMap[1]);
                break;
            case gameType.ragnoRock:
                gapTime = float.Parse(timeMap[0]);
                break;
        }

        // if msgRcv is false, then count the time to retrun msgRcv.
        if (!msgRcv) {
            float offTime = gapTime / 1000f;
            if (passTime > offTime) msgRcv = true;
            else passTime += Time.deltaTime;
        }

        

        if (Input.GetKeyDown(KeyCode.V)) {
            enableEMS = !enableEMS;
        }
        
        if (Input.GetKeyDown(KeyCode.Space)) {
            bothSide = !bothSide;
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            BicepEMS();
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            TricepEMS();
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            BothSideEMS();
        }
        if (Input.GetKeyDown(KeyCode.T)) {
            bHaptic.Play();
            //handBhaptic.Play();
        }
        if (Input.GetKeyDown(KeyCode.Y)) {
            bHaptic.Play();
            BicepEMS();
        }
        if (Input.GetKeyDown(KeyCode.H)){
            handBhaptic.Play();
        }

        // Vive controller Input
        if (touchPad.GetStateDown(SteamVR_Input_Sources.Any)) {
            Debug.Log("GG");
        }
        if (touchPad.GetStateUp(SteamVR_Input_Sources.Any)) {
            Debug.Log("AA");
        }

    }

    void msgHandler(string message) {
        Debug.Log(message);
        // msg example as below
        // 06/04 21:05:56.644 RightController Output Vibration Amp 0.1600 Freq 1.0000 Duration 0.0000
        // seperate the information you need
        string[] eventMessage = message.Split(' ');
        string amp = eventMessage[6];
        string dur = eventMessage[10];
        if (!msgRcvBoxing) {
            if (lastAmp != amp) {
                msgRcvBoxing = true;
                //Debug.Log("GGGGG");
            }
        }
        lastAmp = amp;
        // play around with your device here
        if (mode == stimMode.bothSide) {
            if (enableEMS) {
                if (msgRcvBoxing && msgRcv && eventMessage[2] == "RightController") {
                    if (bothSide) {
                        bHaptic.Play();
                        handBhaptic.Play();
                        BothSideEMS();
                    }
                    else {
                        bHaptic.Play();
                        handBhaptic.Play();
                        BicepEMS();
                    }
                    if (game == gameType.boxing) {
                        if (punch) {
                            msgRcvBoxing = false; // stop listening anymore signal from controller. 
                            //Debug.Log("GOGOGOGOOO");
                        }
                        else {
                            msgRcv = false;
                            passTime = 0;
                        }
                    }
                    else if (game == gameType.ragnoRock) {
                        msgRcv = false;
                        passTime = 0;
                    }

                }

            }
        }
        else if (mode == stimMode.vibra) {
            if (enableEMS) {
                if (msgRcv && eventMessage[2] == "RightController") {
                    if (bothSide) {
                        bHaptic.Play();
                        BothSideEMS();
                    }
                    else {
                        bHaptic.Play();
                        BicepEMS();
                    }
                    if (game == gameType.boxing) {
                        msgRcvBoxing = false; // stop listening anymore signal from controller. 
                        lastAmp = amp;
                    }
                    else if (game == gameType.ragnoRock) {
                        msgRcv = false;
                        passTime = 0;
                    }
                }
            }
        }
        else if (mode == stimMode.airJet) {

        }

    }

    /*void msgHandler2(string message) {
        Debug.Log(message);
        // msg example as below
        // 06/04 21:05:56.644 RightController Output Vibration Amp 0.1600 Freq 1.0000 Duration 0.0000
        // seperate the information you need
        string[] eventMessage = message.Split(' ');
        string amp = eventMessage[6];
        string dur = eventMessage[10];

        // play around with your device here
        if (enableEMS) {
            if (msgRcv) {
                //Debug.Log("EMS ON");
                arduinoEMS.ArduinoWrite("t " + onTime + "\n");

                passTime = 0;
                msgRcv = false; // stop listening anymore signal from controller. 
            }

        }

    }*/
    
    void BicepEMS() {
        arduinoEMS.ArduinoWrite("e " + onTime + "\n");
    }

    void TricepEMS() {
        arduinoEMS.ArduinoWrite("o " + onTime + "\n");
    }

    void BothSideEMS() {
        arduinoEMS.ArduinoWrite("t " + onTime + "\n");
    }
}