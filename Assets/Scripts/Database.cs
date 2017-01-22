using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Database : MonoBehaviour {
    
    private string FILENAME = "db.dat";
    private FileStream fs = null;
    private int curUID;

    public struct row {
        public int UID;
        public string name;
        public TimeSpan time;

        public row(int UID, string name, TimeSpan time)
        {
            this.UID = UID;
            this.name = name;
            this.time = time;
        }
    }

    private List<row> DB; 

    public void initDB()
    {
        DB = new List<row>();
        if(File.Exists(FILENAME))
        {
            curUID = 0;
            string[] lines = File.ReadAllLines(@FILENAME);
            foreach (string line in lines)
            {
                string[] fields = line.Split();
                DB.Add(new row( int.Parse(fields[0]), fields[1], TimeSpan.Parse(fields[2]) ));
                curUID++;
            }
        }
        else
        {
            fs = File.Create(FILENAME);
            curUID = 1;
        }
    }

    public List<row> sortTime()
    {
        List<row> sorted = DB;
        sorted.Sort((x,y) => TimeSpan.Compare(x.time, y.time));
        Debug.Log("Sorted:");
        foreach (row tuple in sorted)
        {
            Debug.Log(tuple.UID + " " + tuple.name + " " + tuple.time);
        }
        return sorted;
    }

    public row getTuple(int UID)
    {
        foreach (row tuple in DB)
        {
            if (tuple.UID == UID)
            {
                Debug.Log("Tuple: " + tuple.UID + " " + tuple.name + " " + tuple.time);
                return tuple;
            }
        }
        Debug.Log("Tuple: UID not Found");
        return new row(-1, null, new TimeSpan(0));
    }

    public void writeEntry(string name, TimeSpan time)
    {
        curUID++;
        DB.Add(new row(curUID, name, time));
        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@FILENAME, true))
        {
            file.WriteLine(string.Format( "{0} {1} {2}", curUID, name, time));
        } 
    }

    public List<row> getDB()
    {
        Debug.Log("DB:");
        foreach (row tuple in DB)
        {
            Debug.Log(tuple.UID + " " + tuple.name + " " + tuple.time);
        }
        return DB;
    }
}
