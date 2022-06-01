using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtonController : MonoBehaviour
{
    public int index = 0;
    private Button button; 
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(menuButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void menuButtonClick()
    {
        if (index == 0)
        {
            SceneManager.LoadScene("GameScene");
        } else if (index == 1)
        {
            SceneManager.LoadScene("BookScene");
        }
    }
}
