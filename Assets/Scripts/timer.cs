using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class timer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    //float timerCount = 0.0F;
    public Text timerText;
    TimeSpan timerCount;
    
	// Update is called once per frame
	void Update () {
        //timerCount += Time.deltaTime;
        timerCount = timerCount.Add(TimeSpan.FromSeconds(Time.deltaTime));
        string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", timerCount.Minutes, timerCount.Seconds, timerCount.Milliseconds);
        timerText.text = timeText;
    }
}
