using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class StopNode : MonoBehaviour {

    private GameObject stopNode;
    private GameObject player;
    private GameObject gm;
    private TimeSpan timer;

    // Use this for initialization
    void Start () {
        gm = GameObject.Find("GameManager");
        stopNode = this.gameObject;
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            writeScore();
            SceneManager.LoadScene(1);
        }
    }

    void writeScore()
    {
        timer = gm.GetComponent<timer>().getTimerCount();
        Debug.Log("hello this is time" + timer.ToString());
        Database DB = getDB();
        DB.writeEntry("testName", timer);
    }

    Database getDB()
    {
        Database DB = gameObject.AddComponent<Database>();
        DB.initDB();
        return DB;
    }
}
