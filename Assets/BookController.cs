using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BookController : MonoBehaviour
{
    private Save save;
    private MonsterGirlDB db;
    public GameObject itemPrefab;
    public AudioClip click;
    private Transform list;
    private GameObject cry;
    private GameObject name;
    private GameObject desc;
    private GameObject preview;
    private int selected;
    // Start is called before the first frame update
    void Start()
    {
        save = new Save();
        save.load();
        db = MonsterGirlDB.getInstance();
        list = GameObject.Find("List").transform;
        cry = GameObject.Find("Cry");
        name = GameObject.Find("Name");
        desc = GameObject.Find("Description");
        preview = GameObject.Find("Preview");
        showBlank();
        foreach (KeyValuePair<int, MonsterGirl> species in db.getAll())
        {
            GameObject item = Instantiate(itemPrefab);
            item.GetComponentInChildren<TextMeshProUGUI>().text = "???";
            if (save.unlocked.Contains(species.Key))
            {
                item.GetComponentInChildren<TextMeshProUGUI>().text = species.Value.name;
            }
            item.GetComponent<ListItemController>().index = species.Key;
            item.transform.SetParent(list);
            item.transform.localScale = Vector2.one;
        }
        GetComponent<AudioSource>().volume = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showItem(int index)
    {
        if (save.unlocked.Contains(index))
        {
            selected = index;
            MonsterGirl musumon = db.get(index);
            TextMeshProUGUI text = name.GetComponent<TextMeshProUGUI>();
            text.text = musumon.name;
            text = desc.GetComponent<TextMeshProUGUI>();
            text.text = musumon.desc;
            preview.GetComponent<Image>().sprite = musumon.sprite;
            cry.SetActive(true);
            //name.SetActive(true);
            desc.SetActive(true);
            preview.SetActive(true);
        } else
        {
            showBlank();
        }
    }

    private void showBlank()
    {
        cry.SetActive(false);
        //name.SetActive(false);
        TextMeshProUGUI text = name.GetComponent<TextMeshProUGUI>();
        text.text = "???";
        desc.SetActive(false);
        preview.SetActive(false);
    }

    public void playCry()
    {
        GetComponent<AudioSource>().PlayOneShot(db.get(selected).cry);
    }

    public void playClick()
    {
        GetComponent<AudioSource>().PlayOneShot(click);
    }
}
