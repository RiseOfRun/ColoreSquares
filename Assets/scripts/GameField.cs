using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Transactions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Diagnostics;
using Random = UnityEngine.Random;

public class GameField : MonoBehaviour
{
    public float Padding = 2;
    public Square SquarePrefab;
    public GameObject FramePrefab;
    public bool CanSpawn = true;
    public int rows = 4;
    public int cols = 4;
    public Square[,] field;
    
    private float cellSize;
    private float space;
    List<int[]> emptyCells= new List<int[]>();
    // Start is called before the first frame update
    void Start()
    {
        if (SwipeDetector.Instance!=null)
        {
            SwipeDetector.Instance.OnSwipe += OnInput;
        }
        GameController.Instance.Field = this;
        GameController.Instance.GameOnPause = false;
        BuildGrid();
        GameController.Instance.StartGame(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnInput(Vector2Int.down);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnInput(Vector2Int.left);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnInput(Vector2Int.right);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnInput(Vector2Int.up);
        }
    }

    private void OnInput(Vector2Int diraction)
    {
        bool moveDone = false;
        foreach (var square in field)
        {
            if (square != null)
            {
                square.CanMerge = true;
            }
        }

        int start = diraction.x > 0 || diraction.y > 0 ? rows - 1 : 0;
        int dir = diraction.x > 0 || diraction.y > 0 ? -1 : 1;
        bool opposite = diraction.x == 0;
        
        for (int i = start+dir; i>=0 && i<rows; i+=dir)
        {
            for (int j = 0; j<rows; j++)
            {

                Vector2Int xy = opposite ? new Vector2Int(j, i) : new Vector2Int(i, j); 
                Square current = Get(xy); 
                if (current == null)
                {
                    continue;
                }
                
                Move(xy,diraction,ref moveDone);
            }
        }
        
        if (moveDone)
        {
            SpawnRandomSquare();
            GameController.Instance.TurnPerformed();
            GameController.Instance.CanBack = true;
        }
        GameController.Instance.CheckGameState();
    }

    private Square Get(Vector2Int pos, out bool found)
    {
        found = false;
        Square toReturn = null;
        if (pos.x < 0 || pos.x > rows - 1 || pos.y < 0 || pos.y > cols - 1) return toReturn;
        toReturn = Get(pos);
        found = true;
        return toReturn;
    }
    private Square Get(Vector2Int pos)
    {
        
        return field[pos.x, pos.y];
    }

    private void Set(Vector2Int pos, Square square)
    {
        field[pos.x, pos.y] = square;
    }

    void Move(Vector2Int currentPosition, Vector2Int diraction, ref bool someMoved)
    {
        Print();
        Square current = Get(currentPosition);
        Square next;
        Vector2Int lastPosition = currentPosition;
        Vector2Int nextPosition = currentPosition + diraction;

        while (nextPosition.x < rows && nextPosition.x>=0 && nextPosition.y < cols && nextPosition.y >=0)
        {
            if (Get(nextPosition)==null)
            {
                currentPosition = nextPosition;
                someMoved = true;
            }
            else
            {
                var nextSquare = Get(nextPosition);
                if (nextSquare.Weight==current.Weight)
                {
                    if (nextSquare.CanMerge)
                    {
                        current.CanMerge = false;
                        current.MergeAfterReachingTarget.Enqueue(nextSquare);
                        GameController.Instance.Score += (int)Mathf.Pow(2, current.Weight);
                            currentPosition = nextPosition;
                        someMoved = true;
                    }
                }

                break;
            }
            nextPosition += diraction;
        }

        current.TargetPosition.Enqueue(currentPosition);
        Set(lastPosition, null);
        Set(currentPosition, current);
    }

    void Print()
    {
        string a = "";
        for (int i = 0; i < field.GetLength(0); i++)
        {
            for (int j = 0; j < field.GetLength(1); j++)
            {
                a += $"({i},{j}) ";
                    a += field[i, j] == null ? "-" : field[i, j].Weight.ToString();
            }

            a += '\n';
        }
        Debug.Log(a);
    }
    

    void BuildGrid()
    {
        field = new Square[rows,cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                var Frame = Instantiate(FramePrefab, transform);
                Frame.transform.localPosition=new Vector2(i,j);
                field[i, j] = null;
            }
        }
        CanSpawn = true;
    }

    public void SpawnRandomSquare()
    {
        List<Vector2Int> pool = new List<Vector2Int>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector2Int position = new Vector2Int(i,j);
                if (Get(position)==null)
                {
                    pool.Add(position);
                }
            }
        }
        int count = pool.Count;
        if (count==0)
        {
            CanSpawn = false;
            return;
        }
        System.Random r = new System.Random();
        Vector2Int randomCell = pool[r.Next(count - 1)];
        SpawnSquare(randomCell);
        
    }

    public void SpawnSquare(Vector2Int position)
    {
        System.Random r = new System.Random();
        Square square = Instantiate(SquarePrefab, transform);
        int number = r.Next(100) > 95 ? 2 : 1;
        square.Weight = number;
        square.transform.localPosition = new Vector2(position.x,position.y);
        Set(position,square);    
    }

    public void SpawnDebugSquare(Vector2Int position)
    {
        Square square = Instantiate(SquarePrefab, transform);
        square.Weight = 10;
        square.transform.localPosition = (Vector2)position;
        Set(position,square);
    }
    
    public string SaveActiveSquares()
    {
        GameController.Instance.GameOnPause = true;
        List<SquareInformation> squares = new List<SquareInformation>();
        Square[] activeSquares = field.Cast<Square>().Where(square => square != null).ToArray();
        foreach (var square in activeSquares)
        {
            squares.Add(square.Dump());
        }
        GameController.Instance.GameOnPause = false;
        return squares.Serialize();

    }

    public void Refresh()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Square>())
            {
                Destroy(child.gameObject);
            }
        }
    }

    public bool Full()
    {
        bool full = true;
        if (field.Cast<Square>().Any(cell => cell == null))
        {
            full = false;
            return full;
        }

        foreach (var cell in field)
        {
            Vector2Int pos = Vector2Int.RoundToInt(cell.transform.localPosition);
            bool found = false;

            Square nextCell=null; 
            nextCell= Get(pos + new Vector2Int(-1, 0),out found);
            if (Square.Compatible(nextCell, cell)) return false;
            nextCell= Get(pos + new Vector2Int(1, 0),out found);
            if (Square.Compatible(nextCell, cell)) return false;
            nextCell= Get(pos + new Vector2Int(0, 1),out found);
            if (Square.Compatible(nextCell, cell)) return false;
            nextCell= Get(pos + new Vector2Int(0, -1),out found);
            if (Square.Compatible(nextCell, cell)) return false;
        }

        return true;
    }
    
}
