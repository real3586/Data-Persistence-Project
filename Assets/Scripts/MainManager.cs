using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using static UnityEngine.Rendering.DebugUI;

public class MainManager : MonoBehaviour
{
    #region//variables
    public static MainManager Instance;
    public Save data;

    public Brick BrickPrefab;
    public GameObject bricks;
    public int LineCount = 6;
    public Rigidbody BallRig;
    public GameObject Ball;
    public GameObject paddle;

    public Text ScoreText;
    public Text GameOverText;
    public Text NameText;
    public Text HighScoreAndName;

    private int m_Points;
    private int highScore = 0;
    private int highScoreCompare;
    public string playerName;
    public string highScoreName;

    public bool m_GameOver = false;

    private Scene _scene;
    private bool correctScene = false;

    int[] color = new[] { 1, 1, 2, 2, 5, 5 };
    #endregion
    private void Awake()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
        ManageInstance();
    }
    private void StartGame()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        // place all the bricks
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = color[i];
            }
        }
        // Set the ball in motion.
        Ball.SetActive(true);
        BallRig.isKinematic = true;
        Ball.transform.position = paddle.transform.position + new Vector3(0, 0.2f, 0);
        Ball.transform.SetParent(paddle.transform);
    }
    IEnumerator GameSequence()
    {
        GameOverText.text = "";

        // start when pressing space bar
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        m_Points = 0;
        ScoreText.text = $"Score : {m_Points}";
        StartGame();

        float randomDirection = Random.Range(-1.0f, 1.0f);
        Vector3 forceDir = new Vector3(randomDirection, 1, 0);
        forceDir.Normalize();
        Ball.transform.SetParent(null);
        BallRig.isKinematic = false;
        BallRig.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);

        if (m_Points == 96)
        {
            // you win the game
            m_GameOver = true;
        }
        
        // wait until game is over
        yield return new WaitUntil(() => m_GameOver);
        
        Debug.Log("Game over");
        highScoreCompare = m_Points;
        /*for (int i = bricks.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(bricks.transform.GetChild(i).gameObject);
        }*/
        HighScore(highScoreCompare);

        // wait for spacebar and then start another game
        
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        Debug.Log("Space pressed");
        m_GameOver = false;

        Debug.Log("Restarted Coroutine");
        StartCoroutine(GameSequence());
        yield return null;
    }
    public void HighScore(int compare)
    {
        if (highScore == 0 || highScore < compare)
        {
            highScore = compare;
            highScoreName = playerName;
            if (string.IsNullOrEmpty(highScoreName))
            {
                highScoreName = "Anonymous";
            }
        }
        else
        {
            return;
        }
        HighScoreAndName.text = "High Score: " + highScore + "  Name: " + highScoreName;
    }
    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.text = "GAME OVER\nPress Space to Restart:";
    }
    public void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }
    void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        _scene = SceneManager.GetActiveScene();
        correctScene = _scene.buildIndex == 1;
        if (correctScene)
        {
            ManageInstance();
            AssignMissing();
            data = new Save();
            data.LoadData();
            StartCoroutine(GameSequence());
        }
    }
    void ManageInstance()
    {
        m_GameOver = false;
        if (Instance != null)
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }
    // use this function to assign values to the variables
    private void AssignMissing()
    {
        Ball = GameObject.Find("Ball");
        BallRig = Ball.GetComponent<Rigidbody>();
        ScoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        GameOverText = GameObject.Find("GameOverText").GetComponent<Text>();
        paddle = GameObject.Find("Paddle");
        HighScoreAndName = GameObject.Find("High Score and Name").GetComponent<Text>();
        NameText = GameObject.Find("NameText").GetComponent<Text>();
        bricks = GameObject.Find("Bricks");

        HighScoreAndName.text = "High Score: " + highScore + "  from: " + highScoreName;
        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Anonymous";
        }
        NameText.text = "Name: " + playerName;
        GameOverText.text = "";
    }
    [System.Serializable] public class Save
    {
        public string saveName;
        public int saveHighScore;
        public void SaveData()
        {
            Save data = new Save();
            data.saveName = Instance.playerName;
            data.saveHighScore = Instance.highScore;

            string json = JsonUtility.ToJson(data);

            File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        }
        public void LoadData()
        {
            string path = Application.persistentDataPath + "/savefile.json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                Save data = JsonUtility.FromJson<Save>(json);

                Instance.highScoreName = data.saveName;
                Instance.highScore = data.saveHighScore;
            }
            if (string.IsNullOrEmpty(Instance.highScoreName))
            {
                Instance.highScoreName = "Anonymous";
            }
        }
    }
}
