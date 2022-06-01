using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipperController : MonoBehaviour
{
    public string control = "Right";
    public int force = 1;
    private KeyCode key;
    private GameController game;
    // Start is called before the first frame update
    void Start()
    {
        key = control == "Right" ? KeyCode.RightArrow : KeyCode.LeftArrow;
        game = GameObject.Find("GameController").GetComponent<GameController>();
#if UNITY_WEBGL
        force *= 3;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (game.state == GameState.PLAYING)
        {
            if (Input.GetKey(key))
            {
                Rigidbody2D body = GetComponent<Rigidbody2D>();
                body.AddForce(new Vector2(force, force), ForceMode2D.Impulse);
            }
        }
    }
}
