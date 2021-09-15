using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public GameObject LosePanel;

    public int Score
    {
        get => score;
        set
        {
            score = value;
            if (value == 2048)
            {
                WinPanel.SetActive(true);
            }

            if (score > BestScore)
            {
                BestScore = score;
                PlayerPrefs.SetInt("BestScore", score);
            }
        }
    }

    public int BestScore = 0;
    public bool CanBack;
    public bool GameOnPause;
    public GameObject WinPanel;
    [FormerlySerializedAs("field")] public GameField Field;

    private int score;

    public int CurrentSeed;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        if (Instance == this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(Instance);
    }

    public void StartGame(bool newGame)
    {
        BestScore = PlayerPrefs.GetInt("BestScore");
        LosePanel.SetActive(false);
        WinPanel.SetActive(false);
        if (newGame)
        {
            CurrentSeed = UnityEngine.Random.Range(0, int.MaxValue);
            PlayerPrefs.SetInt("Random", CurrentSeed);
            PlayerPrefs.SetString("CanBack", "true");
            CanBack = true;
            Score = 0;
            Field.Refresh();
            Field.SpawnRandomSquare();
            Field.SpawnRandomSquare();
            //Field.SpawnDebugSquare(new Vector2Int(0,0));
            //Field.SpawnDebugSquare(new Vector2Int(0,1));
            TurnPerformed();
        }
        else
        {
            Instance.Load();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameOver();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            if (CanBack)
            {
                Instance.Load(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Instance.Load();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Field.Refresh();
        }
    }

    private void GameOver()
    {
        GameOnPause = true;
        LosePanel.SetActive(true);
    }

    public void CheckGameState()
    {
        if (Field.Full())
        {
            GameOver();
        }
    }

    public void TurnPerformed()
    {
        PlayerPrefs.SetString("FieldStatementPrev", PlayerPrefs.GetString("FieldStatement"));
        PlayerPrefs.SetInt("CurrentScorePrev", PlayerPrefs.GetInt("CurrentScore"));
        PlayerPrefs.SetString("FieldStatement", Field.SaveActiveSquares());
        PlayerPrefs.SetInt("CurrentScore", Score);
        PlayerPrefs.SetString("CanBack", "true");
        PlayerPrefs.SetInt("RandomPrev", PlayerPrefs.GetInt("Random"));
        CurrentSeed = UnityEngine.Random.Range(0, int.MaxValue);
        PlayerPrefs.SetInt("Random", CurrentSeed);
    }

    public void Load(bool Undo = false)
    {
        if (BestScore == 0)
        {
            StartGame(true);
        }

        if (Undo)
        {
            if (!Instance.CanBack)
            {
                return;
            }

            PlayerPrefs.SetString("FieldStatement", PlayerPrefs.GetString("FieldStatementPrev"));
            PlayerPrefs.SetInt("Random", PlayerPrefs.GetInt("RandomPrev"));
            PlayerPrefs.SetInt("CurrentScore", PlayerPrefs.GetInt("CurrentScorePrev"));
            Instance.CanBack = false;
            PlayerPrefs.SetString("CanBack", "false");
        }

        LoadSquares();
        Score = PlayerPrefs.GetInt("CurrentScore");
        CurrentSeed = PlayerPrefs.GetInt("Random");
        CanBack = bool.Parse(PlayerPrefs.GetString("CanBack"));
    }

    private void LoadSquares()
    {
        string squares = PlayerPrefs.GetString("FieldStatement");
        List<SquareInformation> activeSquaresInfo = squares.DeSerialize<List<SquareInformation>>();
        Field.Refresh();
        foreach (SquareInformation sq in activeSquaresInfo)
        {
            Vector2 pos = sq.Position;
            Square recSquare = Instantiate(Field.SquarePrefab, Field.transform);
            recSquare.Load(sq);
            Field.field[(int) pos.x, (int) pos.y] = recSquare;
        }
    }
}