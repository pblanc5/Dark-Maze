using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
    float timerCount = 0.0F;
    public Text timerText;
	// Update is called once per frame
	void Update () {
        timerCount += Time.deltaTime;
        timerText.text = "Time: " + Mathf.Round(timerCount);
	}
}
