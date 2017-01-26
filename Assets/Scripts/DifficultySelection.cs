using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultySelection : MonoBehaviour {

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Play")
        {
            SceneManager.LoadScene(2);
        }
        else if (other.gameObject.name == "Scoreboard")
        {
            SceneManager.LoadScene(1);
        }
        else if (other.gameObject.name == "Back")
        {
            SceneManager.LoadScene(0);
        }
    }

}
