using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGirl
{
    public int id;
    public string name;
    public string desc;
    public int tier;
    public AudioClip cry;
    public Sprite sprite;

    public MonsterGirl(int index, string speciesName, string description, int t)
    {
        id = index;
        name = speciesName;
        desc = description;
        tier = t;
        cry = Resources.Load<AudioClip>("cry_" + index);
        Texture2D tex = Resources.Load<Texture2D>("sprite_" + index);
        sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }
}
