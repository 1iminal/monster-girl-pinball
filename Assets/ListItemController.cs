using UnityEngine;
using UnityEngine.UI;

public class ListItemController : MonoBehaviour
{
    public int index = 0;
    private BookController book;
    // Start is called before the first frame update
    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(itemClick);
        book = GameObject.Find("BookController").GetComponent<BookController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void itemClick()
    {
        book.playClick();
        book.showItem(index);
    }
}
