using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.U2D;

public static class GameState
{
    public static int PLAYING = 0;
    public static int CUTSCENE = 1;
    public static int GAME_OVER = 2;
    public static int PAUSE = 3;
}

public class CutsceneState
{
    public static int PENTAGRAM = 0;
    public static int SUMMON = 1;
    public static int RESULT = 2;
}

public class GameController : MonoBehaviour
{
    public int state;
    private int score;
    private int balls;
    private TextMeshProUGUI tipLabel;
    private TextMeshProUGUI scoreLabel;
    private TextMeshProUGUI ballsLabel;
    private TextMeshProUGUI announceLabel;
    private float announceTime;
    private static string[] tip = new string[] { "[T] - show/hide this tip", "[Space] - launch the orb\n[Left/Right] - control flippers\n[A/D] - push the table\n[Esc] - go to main menu\n[T] - show/hide this tip" };
    private static Vector3 ballDefaultPos = new Vector3(6.7f, -4.25f, 0);
    private int currentTip = 1;
    private GameObject ball;
    private GameObject camera;
    private bool plungerEnabled = false;
    private GameObject gate;
    private bool gateClosed = true;
    private GameObject pentagram;
    private int cutsceneState;
    private float cutsceneStateLeft;
    private ParticleSystem particleSystem;
    private Vector2 savedVelocity;
    private float savedAngularVelocity;
    private SpriteRenderer sprite;
    private SpriteRenderer tint;
    private AudioSource audioSource;
    private float sinceLastPush;
    private MonsterGirlDB db;
    private int summoned;
    private Save save;
    private static Dictionary<int, int> scoreTable = new Dictionary<int, int>()
    {
        { 1, 15 },
        { 2, 30 },
        { 3, 60 },
    };
    private AudioSource bgm;
    // Start is called before the first frame update
    void Start()
    {
        save = new Save();
        save.load();
        score = 0;
        balls = 3;
        tipLabel = GameObject.Find("Tip").GetComponent<TextMeshProUGUI>();
        scoreLabel = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
        ballsLabel = GameObject.Find("Balls").GetComponent<TextMeshProUGUI>();
        ballsLabel.text = "ORBS: " + balls;
        announceLabel = GameObject.Find("Announce").GetComponent<TextMeshProUGUI>();
        announceLabel.color = new Color(255, 255, 255, 0);
        announceTime = 0;
        ball = GameObject.Find("Ball");
        camera = GameObject.Find("Main Camera");
        gate = GameObject.Find("Gate");
        closeGate(false);
        pentagram = GameObject.Find("pentagram");
        hidePentagram();
        particleSystem = GameObject.Find("Center sprite").GetComponent<ParticleSystem>();
        sprite = GameObject.Find("Center sprite").GetComponent<SpriteRenderer>();
        setSpriteAlpha(sprite, 0);
        tint = GameObject.Find("Center tint").GetComponent<SpriteRenderer>();
        setupTint(); 
        setSpriteAlpha(tint, 0);
        audioSource = GameObject.Find("AudioController").GetComponent<AudioSource>();
        audioSource.volume = 0.5f;
        sinceLastPush = 0;
        db = MonsterGirlDB.getInstance();
        bgm = GameObject.Find("BGM").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            currentTip = currentTip == 1 ? 0 : 1;
            tipLabel.text = tip[currentTip];
        }
        if (state == GameState.PLAYING)
        {
            camera.transform.position = new Vector3(ball.transform.position.x, ball.transform.position.y, camera.transform.position.z);
            if (Input.GetKeyDown(KeyCode.Pause))
            {
                state = GameState.PAUSE;
                savedVelocity = ball.GetComponent<Rigidbody2D>().velocity;
                savedAngularVelocity = ball.GetComponent<Rigidbody2D>().angularVelocity;
                ball.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                ball.GetComponent<Rigidbody2D>().angularVelocity = 0;
                ball.GetComponent<Rigidbody2D>().isKinematic = true;
            }
            if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) && sinceLastPush >= 0.5)
            {
                Vector2 force = new Vector2(0, 0);
                if (Input.GetKeyDown(KeyCode.A)) {
                    force.x = -5;
                } else
                {
                    force.x = 5;
                }
                ball.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
                audioSource.PlayOneShot(audioSource.gameObject.GetComponent<AudioController>().push);
                sinceLastPush = 0;
            }
            sinceLastPush += Time.deltaTime;
        }
        else if (state == GameState.PAUSE)
        {
            if (Input.GetKeyDown(KeyCode.Pause))
            {
                ball.GetComponent<Rigidbody2D>().isKinematic = false;
                ball.GetComponent<Rigidbody2D>().velocity = savedVelocity;
                ball.GetComponent<Rigidbody2D>().angularVelocity = savedAngularVelocity;
                state = GameState.PLAYING;
            }
        }
        if (state == GameState.GAME_OVER)
        {
            if (Input.anyKey)
            {
                SceneManager.LoadScene("MenuScene");
            }
            return;
        }
        if (state == GameState.CUTSCENE)
        {
            cutsceneStateLeft -= Time.deltaTime;
            if (cutsceneState == CutsceneState.PENTAGRAM)
            {
                if (cutsceneStateLeft > 0)
                {
                    increasePentagramAlpha();
                } else
                {
                    cutsceneState = CutsceneState.SUMMON;
                    particleSystem.Play();
                    cutsceneStateLeft = 3;

                }
            } else if (cutsceneState == CutsceneState.SUMMON)
            {
                if (cutsceneStateLeft > 0)
                {
                    setSpriteAlpha(tint, tint.color.a + Time.deltaTime);
                } else
                {
                    setSpriteAlpha(sprite, 1);
                    setSpriteAlpha(tint, 0);
                    particleSystem.Stop();
                    hidePentagram();
                    deactivateCandles();
                    addBall();
                    addScore(scoreTable[db.get(summoned).tier], false);
                    audioSource.PlayOneShot(db.get(summoned).cry);
                    announce(db.get(summoned).name + " summoned", 2);
                    if (!save.unlocked.Contains(summoned))
                    {
                        save.unlocked.Add(summoned);
                        save.save();
                    }
                    cutsceneState = CutsceneState.RESULT;
                    cutsceneStateLeft = 3;
                    audioSource.clip = audioSource.gameObject.GetComponent<AudioController>().spawn;
                    audioSource.Play();
                }
            } else if (cutsceneState == CutsceneState.RESULT)
            {
                if (cutsceneStateLeft <= 0)
                {
                    setSpriteAlpha(sprite, 0);
                    state = GameState.PLAYING;
                    ball.GetComponent<Rigidbody2D>().isKinematic = false;
                    ball.GetComponent<Rigidbody2D>().velocity = savedVelocity;
                    ball.GetComponent<Rigidbody2D>().angularVelocity = savedAngularVelocity;
                    bgm.Play();
                }
            }
        }
        if (announceTime > 0)
        {
            announceTime -= Time.deltaTime;
        } else
        {
            if (announceLabel.color.a > 0)
            {
                announceLabel.color = new Color(1f, 1f, 1f, (announceLabel.color.a - Time.deltaTime * 1000)/255);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && plungerEnabled)
        {
            ball.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 75), ForceMode2D.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            saveHighscore();
            SceneManager.LoadScene("MenuScene");
        }
    }

    public void addScore(int add, bool playSfx)
    {
        score += add;
        scoreLabel.text = "SCORE: " + score;
        if (playSfx)
        {
            audioSource.PlayOneShot(audioSource.gameObject.GetComponent<AudioController>().candleHit);
        }
    }

    public void ballLost()
    {
        balls--;
        ballsLabel.text = "ORBS: " + balls;
        ball.transform.position = ballDefaultPos;
        closeGate(false);
        if (balls < 1)
        {
            state = GameState.GAME_OVER;
            announce("GAME OVER", -1);
            saveHighscore();
        } else
        {
            announce("ORB LOST", 1);
            audioSource.PlayOneShot(audioSource.gameObject.GetComponent<AudioController>().ballLost);
        }
    }

    public void addBall()
    {
        balls++;
        ballsLabel.text = "ORBS: " + balls;
    }

    public void announce(string text, float time)
    {
        announceLabel.text = text;
        announceLabel.color = new Color(255, 255, 255, 255);
        announceTime = time;
    }

    public void enablePlunger(bool enable)
    {
        plungerEnabled = enable;
    }

    public void closeGate(bool close)
    {
        if (gateClosed == close)
        {
            return;
        }
        gate.SetActive(close);
        gateClosed = close;
    }

    public void candleActivated()
    {
        GameObject candlesParent = GameObject.Find("candles");
        for (int i = 0; i < candlesParent.transform.childCount; i++)
        {
            CandleController candle = candlesParent.transform.GetChild(i).gameObject.GetComponentInChildren<CandleController>();
            if (!candle.activated)
            {
                return;
            }
        }
        roll();
    }

    public void hidePentagram()
    {
        for (int i = 0; i < pentagram.transform.childCount - 1; i++)
        {
            SpriteRenderer shape = pentagram.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>();
            shape.color = new Color(1, 1, 1, 0);
        }
        SpriteShapeRenderer circle = pentagram.GetComponentInChildren<SpriteShapeRenderer>();
        circle.color = new Color(1, 1, 1, 0);
    }

    public void increasePentagramAlpha()
    {
        for (int i = 0; i < pentagram.transform.childCount - 1; i++)
        {
            SpriteRenderer shape = pentagram.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>();
            shape.color = new Color(1, 1, 1, shape.color.a + Time.deltaTime);
        }
        SpriteShapeRenderer circle = pentagram.GetComponentInChildren<SpriteShapeRenderer>();
        circle.color = new Color(1, 1, 1, circle.color.a + Time.deltaTime);
    }

    private void roll()
    {
        summoned = Random.Range(1, db.count);
        sprite.sprite = db.get(summoned).sprite;
        setupTint();
        showCutscene();
    }

    private void showCutscene()
    {
        state = GameState.CUTSCENE;
        cutsceneState = CutsceneState.PENTAGRAM;
        cutsceneStateLeft = 1;
        // save ball velocity and freeze ball
        savedVelocity = ball.GetComponent<Rigidbody2D>().velocity;
        savedAngularVelocity = ball.GetComponent<Rigidbody2D>().angularVelocity;
        ball.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        ball.GetComponent<Rigidbody2D>().angularVelocity = 0;
        ball.GetComponent<Rigidbody2D>().isKinematic = true;
        // set camera position
        GameObject center = GameObject.Find("Center");
        camera.transform.position = new Vector3(center.transform.position.x, center.transform.position.y, camera.transform.position.z);
        audioSource.clip = audioSource.gameObject.GetComponent<AudioController>().summon;
        audioSource.Play();
        bgm.Pause();
    }

    private void deactivateCandles()
    {
        Transform candlesParent = GameObject.Find("candles").transform;
        CandleController candle;
        for (int i = 0; i < candlesParent.childCount; i++)
        {
            candle = candlesParent.GetChild(i).gameObject.GetComponentInChildren<CandleController>();
            candle.setActivated(false);
        }
    }

    private void setSpriteAlpha(SpriteRenderer sprite, float a)
    {
        sprite.color = new Color(1, 1, 1, a);
    }

    private void setupTint()
    {
        Texture2D sourceTex = sprite.sprite.texture;
        Texture2D tex = new Texture2D(sourceTex.width, sourceTex.height);
        for (int y = 0; y < sourceTex.height; y++)
        {
            for (int x = 0; x < sourceTex.width; x++)
            {
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, sourceTex.GetPixel(x, y).a));
                //tex.SetPixel(x, y, sourceTex.GetPixel(x, y));
            }
        }
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        tint.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    private void saveHighscore()
    {
        Save save = new Save();
        save.load();
        if (score > save.highscore)
        {
            save.highscore = score;
            save.save();
        }
    }
}
