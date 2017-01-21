using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

}
