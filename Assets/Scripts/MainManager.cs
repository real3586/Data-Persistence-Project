using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
using TMPro;

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
    public bool DevCheat = false;

    private void Start()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
        ManageInstance();
    }
    void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        _scene = SceneManager.GetActiveScene();
        correctScene = _scene.buildIndex == 1;
        if (correctScene)
        {
            ManageInstance();
            AssignMissing();
            TextManage();
            data = new Save();
            data.LoadData();
            StartCoroutine(GameSequence());
        }
        else
        {
            data.SaveData();
            playerName = "Anonymous";
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
    private void StartGame()
    {
        // destroy remaining bricks
        for (int i = bricks.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(bricks.transform.GetChild(i).gameObject);
        }

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
        // puts ball above paddle
        Ball.SetActive(true);
        BallRig.isKinematic = true;
        Ball.transform.position = paddle.transform.position + new Vector3(0, 0.2f, 0);
        Ball.transform.SetParent(paddle.transform);
    }
    IEnumerator GameSequence()
    {
        // start when pressing space bar
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        m_Points = 0;
        ScoreText.text = $"Score : {m_Points}";
        StartGame();

        // ball in motion
        float randomDirection = Random.Range(-1.0f, 1.0f);
        Vector3 forceDir = new Vector3(randomDirection, 1, 0);
        forceDir.Normalize();
        Ball.transform.SetParent(null);
        BallRig.isKinematic = false;
        BallRig.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);

        // wait until game is over
        yield return new WaitUntil(() => m_Points == 96 || m_GameOver);
        StartCoroutine(GameOverSequence());
    }
    IEnumerator GameOverSequence()
    {
        if (m_Points == 96)
        {
            // freeze ball and also hide it
            BallRig.isKinematic = true;
            Ball.SetActive(false);
            GameOver(true);
        }

        highScoreCompare = m_Points;
        for (int i = bricks.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(bricks.transform.GetChild(i).gameObject);
        }
        HighScore(highScoreCompare);

        // wait for spacebar and then start another game
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        m_GameOver = false;
        GameOverText.text = " ";

        StartCoroutine(GameSequence());
        yield return null;
    }
    public void HighScore(int compare)
    {
        if (highScore <= compare)
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
    public void GameOver(bool win)
    {
        GameOverText.text = win ? "YOU WIN\nPress Space to Restart" : "GAME OVER\nPress Space to Restart:";
        m_GameOver = true;
    }
    public void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
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
    }
    private void TextManage()
    {
        // changes PLSDevCheat to hAcKeR
        playerName = playerName == "PLSDevCheat" ? "hAcKeR" : playerName;

        // turns on or off DevCheat depending on name
        DevCheat = playerName == "hAcKeR" ? true : false;

        // changes to Anon if there is no name
        playerName = string.IsNullOrEmpty(playerName) ? "Anonymous" : playerName;

        NameText.text = "Name: " + playerName;
        GameOverText.text = "";
    }
    [System.Serializable] public class Save
    {
        public string saveHighName;
        public int saveHighScore;
        public void SaveData()
        {
            Save data = new Save();
            data.saveHighName = Instance.highScoreName;
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

                Instance.highScore = data.saveHighScore;
                if (string.IsNullOrEmpty(data.saveHighName) || data.saveHighName == "Anonymous")
                {
                    Instance.highScoreName = "Anonymous";
                }
                else
                {
                    Instance.highScoreName = data.saveHighName;
                }
            }
            Instance.HighScoreAndName.text = "High Score: " + Instance.highScore + "  from: " + Instance.highScoreName;
        }
    }
}
