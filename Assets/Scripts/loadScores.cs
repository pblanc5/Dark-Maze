using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class loadScores : MonoBehaviour {
    // Use this for initialization
    void Start () {
        List<Database.row> scoreList = getScores();
        int count = scoreList.Count;
        for (int i = 0; i <= 8; i++)
        {
            if (i >= count)
            {
                break;
            }
            else
            {
                writeScore(scoreList[i], i);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    List<Database.row> getScores()
    {
        Database DB = gameObject.AddComponent<Database>();
        List<Database.row> scoreList;
        DB.initDB();
        scoreList = DB.sortTime();
        return scoreList;
    }

    void writeScore(Database.row row, int position)
    {
        string name = row.name;
        TimeSpan time = row.time;
        string line = (position + 1).ToString() + ". " + name + " " + time.Minutes + ":" + time.Seconds + ":" + time.Milliseconds;
        string objName = "Name" + (position + 1).ToString();
        GameObject.Find(objName).GetComponent<UnityEngine.UI.Text>().text = line;
    }
}
