using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

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
            
            if (score>BestScore)
            {
                BestScore = score;
                PlayerPrefs.SetInt("BestScore",score);
            }
        }
    }
    public int BestScore=0;
    public bool CanBack;
    public bool GameOnPause;
    public GameObject WinPanel;
    [FormerlySerializedAs("field")] public GameField Field;

    private int score;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
            return;
        }
        
        if (Instance==this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(Instance);
    }

    public void StartGame(bool newGame)
    {
        BestScore = PlayerPrefs.GetInt("BestScore");
        if (newGame)
        {
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

    public void GameOver()
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
    }

    public void TurnBackrolled()
    {
        Load(true);
        //Field.SpawnRandomSquare();
    }
    public void Load(bool Undo = false)
    {
        if (BestScore==0)
        {
            StartGame(true);
        }
        string FieldStatement = "FieldStatement"; 
        string score = "CurrentScore";
        if (Undo)
        {
            if (!Instance.CanBack)
            {
                return;
            }
            PlayerPrefs.SetString("FieldStatement", PlayerPrefs.GetString("FieldStatementPrev"));
            PlayerPrefs.SetInt("CurrentScore", PlayerPrefs.GetInt("CurrentScorePrev"));
            Instance.CanBack= false;
        }
        else
        {
            CanBack = true;
        }
        string squares = PlayerPrefs.GetString(FieldStatement);
        List<SquareInformation> activeSquaresInfo = squares.DeSerialize<List<SquareInformation>>();
        Field.Refresh();
        foreach (var sq in activeSquaresInfo)
        {
            Vector2 pos = sq.Position;
            Square recSquare = Instantiate(Field.SquarePrefab, Field.transform);
            recSquare.Load(sq);
            Field.field[(int) pos.x, (int) pos.y] = recSquare;
        }
        Score = PlayerPrefs.GetInt(score);
    }
}
