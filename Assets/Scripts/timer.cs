﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class timer : MonoBehaviour {


    //float timerCount = 0.0F;
    public Text timerText;
    TimeSpan timerCount;

    private bool finished;
    
	// Use this for initialization
	void Start ()
    {
        finished = false;
	}

	// Update is called once per frame
	void Update () {
        //timerCount += Time.deltaTime;
        if (!finished)
            timerCount = timerCount.Add(TimeSpan.FromSeconds(Time.deltaTime));
        else
            timerText.color = new Color(100, 200, 100);
        string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", timerCount.Minutes, timerCount.Seconds, returnShortMS(timerCount.Milliseconds));
        timerText.text = timeText;
    }

    public void StopTimer()
    {
        finished = true;
    }

    string returnShortMS (int milliseconds)
    {
        string strMilliseconds = milliseconds.ToString();
        if (strMilliseconds.Length > 2)
            strMilliseconds = strMilliseconds.Substring(0, 2);
        else if (strMilliseconds.Length == 1)
            strMilliseconds = strMilliseconds.Substring(0, 1) + "0";
        else if (strMilliseconds.Length == 0)
            strMilliseconds = "00";
        return strMilliseconds;
    }
}
