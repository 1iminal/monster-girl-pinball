using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CandleController : MonoBehaviour
{
    public bool activated = true;
    public float frequency = 0.000001f;
    private float sinceLast = 0;
    private bool state = false;
    private GameController game;
    private Light2D light;
    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.Find("GameController").GetComponent<GameController>();
        light = transform.parent.GetChild(4).GetComponent<Light2D>();
        setActivated(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            sinceLast -= Time.deltaTime;
            if (sinceLast <= 0)
            {
                sinceLast = frequency;
                state = !state;
                if (state)
                {
                    Transform child = transform.parent.GetChild(2);
                    child.localScale = new Vector3(child.localScale.x + 0.1f, child.localScale.y + 0.1f, child.localScale.z + 0.1f);
                    child = transform.parent.GetChild(3);
                    child.localScale = new Vector3(child.localScale.x - 0.1f, child.localScale.y - 0.1f, child.localScale.z - 0.1f);
                    light.pointLightOuterRadius += 0.2f;
                } else
                {
                    Transform child = transform.parent.GetChild(2);
                    child.localScale = new Vector3(child.localScale.x - 0.1f, child.localScale.y - 0.1f, child.localScale.z - 0.1f);
                    child = transform.parent.GetChild(3);
                    child.localScale = new Vector3(child.localScale.x + 0.1f, child.localScale.y + 0.1f, child.localScale.z + 0.1f);
                    light.pointLightOuterRadius -= 0.2f;
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        game.addScore(1, true);
        setActivated(true);
    }

    public void setActivated(bool activate)
    {
        if (activated == activate)
        {
            return;
        }
        transform.parent.GetChild(2).gameObject.SetActive(activate);
        transform.parent.GetChild(3).gameObject.SetActive(activate);
        transform.parent.GetChild(4).gameObject.SetActive(activate);
        activated = activate;
        if (activated)
        {
            game.candleActivated();
        }
    }
}
