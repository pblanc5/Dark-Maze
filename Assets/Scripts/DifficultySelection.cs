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
        }
        else if (other.gameObject.name == "MediumText")
        {
            medium = true;
        }
        else if (other.gameObject.name == "HardText")
        {
            hard = true;
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
