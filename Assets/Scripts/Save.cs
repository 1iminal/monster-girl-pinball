using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Save
{
    public int highscore = 0;
    public List<int> unlocked = new List<int>();

    public Save() { }

    public void save()
    {
        using (StreamWriter file = new StreamWriter(Application.persistentDataPath + "/save.json"))
        {
            file.Write(JsonUtility.ToJson(this));
        }
    }

    public void load()
    {
        try
        {
            using (StreamReader file = new StreamReader(Application.persistentDataPath + "/save.json"))
            {
                string s = file.ReadToEnd();
                Save save = JsonUtility.FromJson<Save>(s);
                highscore = save.highscore;
                unlocked = save.unlocked;
            }
        } catch
        {
            save();
        }
    }
}