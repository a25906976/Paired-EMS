using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateAngle : MonoBehaviour {
    public GameObject hand;
    public GameObject elbow;
    private float maxAngle;
    private Vector3 previousPosition;
    private Vector3 elbowPosition;
    private float time = 3f; //先設定為3秒
    private float elapsedTime;
    private bool start;
    // Start is called before the first frame update
    void Start() {
        maxAngle = 0f;
        previousPosition = hand.transform.position;
        elapsedTime = 0f;
        start = false;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            start = true;
            previousPosition = hand.transform.position;
            elbowPosition = elbow.transform.position;
        }
        if (start == true) {
            if (elapsedTime <= time) {
                Angle();
            }
            else {
                // Output the maximum distance traveled
                Debug.Log("Max Distance: " + maxAngle + " units");
                // Reset the variables for a new measurement
                maxAngle = 0f;
                elapsedTime = 0f;
                start = false;
            }
        }
        //call other function to compare current length and previous length   
    }

    void Angle() {
        elapsedTime += Time.deltaTime;
        float ang = Vector3.Angle(previousPosition - elbowPosition, hand.transform.position - elbow.transform.position);
        if (ang > maxAngle)
            maxAngle = ang;
    }
}