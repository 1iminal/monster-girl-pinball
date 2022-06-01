using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlungerController : MonoBehaviour
{
    private GameController game;
    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.Find("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        game.enablePlunger(true);
    }
    void OnTriggerExit2D(Collider2D collider)
    {
        game.enablePlunger(false);
    }
}
