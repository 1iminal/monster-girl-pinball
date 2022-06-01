using UnityEngine;
using TMPro;

public class HighscoreController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Save save = new Save();
        save.load();
        TextMeshProUGUI highscore = GameObject.Find("Highscore").GetComponent<TextMeshProUGUI>();
        highscore.text = "Highscore: " + save.highscore;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
