using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultySelection : MonoBehaviour {
    public bool easy = false;
    public bool medium = false;
    public bool hard = false;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "EasyText")
        {
            easy = true;
            SceneManager.LoadScene(3);
        }
        else if (other.gameObject.name == "MediumText")
        {
            medium = true;
            SceneManager.LoadScene(2);
        }
        else if (other.gameObject.name == "HardText")
        {
            hard = true;
            SceneManager.LoadScene(4);
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
