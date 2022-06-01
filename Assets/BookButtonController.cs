using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BookButtonController : MonoBehaviour
{
    public int index = 0;
    private BookController book;
    // Start is called before the first frame update
    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(bookButtonClick);
        book = GameObject.Find("BookController").GetComponent<BookController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void bookButtonClick()
    {
        if (index == 0)
        {
            SceneManager.LoadScene("MenuScene");
        } else if (index == 1)
        {
            book.playCry();
        }
    }
}
