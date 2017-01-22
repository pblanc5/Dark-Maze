using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StopNode : MonoBehaviour {

    private GameObject stopNode;
    private GameObject player;

	// Use this for initialization
	void Start () {
        stopNode = this.gameObject;
        player = GameObject.FindGameObjectWithTag("player");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(0);
    }
}
