using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public int totalIslands;
        public int currentLevelID;
        public float levelViewSize = 36;

        [System.Serializable]
        public class Level
        {
            public float islandViewSize;
            [HideInInspector]
            public Transform island;
            [HideInInspector]
            public Transform spawnPoint;
            [HideInInspector]
            public Transform islandCenter;
            [HideInInspector]
            public UIIslandDisplay uIDisplay;
        }
        public Level[] levels;
    }
    public LevelData levelData;

    [System.Serializable]
    public class UI
    {
        public GameObject menuPanel;
        public GameObject winPanel;
        public GameObject lostPanel;
        public Text scoreText;
        public Text totalScoreText1, totalScoreText2;
    }
    public UI uI;

    public static GameManager instance;

    public string thisLevel;
    public string nextLevel;

    public Transform levelCentre;
    public ParticleSystem spawnEffect;
    public Animator transitionAnim;
    public GameObject levelBeginAnim;

   [HideInInspector]
    public int score;
    [SerializeField]
    private Player player;
    private CameraFollow cameraFollow;

    private void Awake()
    {
        // Set Islands Info
        int islandNum = 1;
        foreach(LevelData.Level island in levelData.levels)
        {
            island.island = GameObject.Find("grass " + islandNum.ToString()).transform;
            island.islandCenter = GameObject.Find("center " + islandNum.ToString()).transform;
            island.spawnPoint = GameObject.Find("spawn point " + islandNum.ToString()).transform;

            island.uIDisplay = GameObject.Find("levelUI " + islandNum.ToString()).GetComponent<UIIslandDisplay>();

            islandNum++;
        }

        Time.timeScale = 1;   

        instance = this;

        cameraFollow = FindObjectOfType<CameraFollow>();

        levelData.totalIslands = levelData.levels.Length - 1;

        score = 0;

        cameraFollow.target = levelCentre;
        cameraFollow.viewSize = levelData.levelViewSize;    

        // Set UI
        SetUIPanels(false, false, true, true);

        uI.totalScoreText1.text = PlayerPrefs.GetInt("Score", 0).ToString();
        uI.totalScoreText2.text = PlayerPrefs.GetInt("Score", 0).ToString();

        UpdateIslandStatus();
    }

    private void Update()
    {
        if (levelData.levels[levelData.currentLevelID].island.childCount <= 0)
        {
            spawnEffect.transform.position = player.currentSaw.position;
            spawnEffect.Play();

            player.gameObject.SetActive(false);

            if (levelData.currentLevelID >= levelData.totalIslands)
            {
                cameraFollow.viewSize = levelData.levelViewSize;
                cameraFollow.target = levelCentre;

                levelData.currentLevelID++;

                UpdateIslandStatus();

                PlayerWon();
            }
            else
            {
                player.Close();

                cameraFollow.target = levelData.levels[levelData.currentLevelID].islandCenter;
                cameraFollow.viewSize = levelData.levels[levelData.currentLevelID].islandViewSize;

                levelData.currentLevelID++;

                UpdateIslandStatus();

                Invoke("BeginLevel", 3);
            }
        }
    }

    private void SetUIPanels(bool win, bool lost, bool menu, bool game)
    {
        uI.winPanel.SetActive(win);
        uI.lostPanel.SetActive(lost);
        uI.menuPanel.SetActive(menu);
    } 

    private void UpdateIslandStatus()
    {
        int IDCount = 0;

        foreach (LevelData.Level level in levelData.levels)
        {
            UIIslandDisplay.State newState = UIIslandDisplay.State.inactive;

            if (IDCount == levelData.currentLevelID)
                newState = UIIslandDisplay.State.active;
            else if (IDCount < levelData.currentLevelID)
                newState = UIIslandDisplay.State.completed;

            level.uIDisplay.SetInfo(newState);

            IDCount++;
        }
    }

    private void BeginLevel()
    {
        Time.timeScale = 1;

        player.transform.position = levelData.levels[levelData.currentLevelID].spawnPoint.position;

        cameraFollow.target = levelData.levels[levelData.currentLevelID].islandCenter;
        cameraFollow.viewSize = levelData.levels[levelData.currentLevelID].islandViewSize;

        Invoke("SpawnPlayer", 3);
    }

    private void SpawnPlayer()
    {
        spawnEffect.transform.position = levelData.levels[levelData.currentLevelID].spawnPoint.position;
        spawnEffect.Play();

        player.gameObject.SetActive(true);
        player.Spawn();

        cameraFollow.target = player.sawA;
        cameraFollow.viewSize = 7;
    }

    private void LoadNextLevel()
    {
        transitionAnim.SetTrigger("won");

        Invoke("LoadNextScaled", 1);
    }

    private void LoadNextScaled()
    {
        SceneManager.LoadScene(nextLevel);
    }

    private void PlayerWon()
    {
        uI.winPanel.SetActive(true);

        PlayerPrefs.SetInt("Score", score);

        SetUIPanels(true, false, false, false);

        PlayerPrefs.GetString("LastLevel", nextLevel);

        Invoke("LoadNextLevel", 2);
    }

    public void PlayerLost()
    {
        Time.timeScale = 0;

        SetUIPanels(false, true, false, true);
    }

    public void Reload()
    {
        PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score", 0) + score);

        SceneManager.LoadScene(thisLevel);
    }

    public void Retry()
    {
        Time.timeScale = 1;

        SetUIPanels(false, false, false, true);
    }

    public void AddScore()
    {
        score++;
        uI.scoreText.text = score.ToString();
    }

    public void StartGame()
    {
        levelBeginAnim.SetActive(true);

        Invoke("BeginLevel", 3);

        SetUIPanels(false, false, false, true);
    }
}
