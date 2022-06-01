using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class MonsterGirlDB
{
    private static MonsterGirlDB instance;
    private static Dictionary<int, MonsterGirl> db;
    public int count;

    private MonsterGirlDB()
    {
        TextAsset json = Resources.Load<TextAsset>("db");
        Dictionary<string, string>[] data = JsonConvert.DeserializeObject<Dictionary<string, string>[]>(json.text);
        db = new Dictionary<int, MonsterGirl>();
        for (int i = 0; i < data.Length; i++)
        {
            int id = int.Parse(data[i]["id"]);
            string name = data[i]["name"];
            string desc = data[i]["desc"];
            int tier = int.Parse(data[i]["tier"]);
            db.Add(id, new MonsterGirl(id, name, desc, tier));
        }
        count = db.Count;
    }

    public static MonsterGirlDB getInstance()
    {
        if (instance == null)
        {
            instance = new MonsterGirlDB();
        }
        return instance;
    }

    public MonsterGirl get(int id)
    {
        return db[id];
    }

    public Dictionary<int, MonsterGirl> getAll()
    {
        return db;
    }
}
