using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Transactions;
using TreeEditor;
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
    public bool CanMove = true;
    public int rows = 4;
    public int cols = 4;
    private float cellSize;
    private float space;
    Square[,] field;
    List<int[]> emptyCells= new List<int[]>();
    
    
    // Start is called before the first frame update
    void Start()
    {
        BuildGrid();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SpawnRandomSquare();
            // SpawnRandomCell();
        }

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
        // int start = diraction.x > 0 || diraction.y < 0 ? rows-1 : 0;
        // int dir = diraction.x > 0 || diraction.y < 0 ? -1 : 1;
        //
        // for (int i = start+dir; i >= 0 && i < rows; i+=dir)
        // {
        //     for (int j = 0; j < rows; j++)
        //     {
        //         bool row = diraction.x == 0;
        //         Cell currentCell = !row ? field[j,i] : field[i,j];
        //         if (currentCell.empty)
        //         {
        //             continue;
        //         }
        //         
        //         Cell nextCell = null;
        //         for (int k = i-dir; k >=0 && k<rows; k-=dir)
        //         {
        //             row = diraction.x == 0;
        //             Cell tmp = row ? field[k,j] : field[j,k];
        //             if (tmp.empty)
        //             {
        //                 nextCell = tmp;
        //                 continue;
        //             }
        //             if (!tmp.HasMerged)
        //             {
        //                 break;
        //                 nextCell = tmp;
        //             }
        //         }
        //         if (nextCell != null)
        //         { 
        //             MoveTo(currentCell,nextCell);
        //         }
        //     }
        // }

        //     bool isOpposite = ;
        //
        //     //for x in grid:
        //     //  for y in grid:
        //     //    MoveCell(coords, direction)
        // }
        //
        //
        //
        // private Cell Get(Vector2Int pos){}
        // private void Set(Vector2Int pos, Cell cell){}
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
        bool Opposite = diraction.x == 0;
        
        for (int i = start+dir; i>=0 && i<rows; i+=dir)
        {
            for (int j = 0; j<rows; j++)
            {

                Vector2Int xy = Opposite ? new Vector2Int(j, i) : new Vector2Int(i, j); 
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
            CanMove = true;
        }
        else
        {
            CanMove = false;
        }
    }

    private Square Get(Vector2Int pos)
    {
        return field[pos.x, pos.y];
    }

    private void Set(Vector2Int pos, Square square)
    {
        field[pos.x, pos.y] = square;
    }
    void MoveTo(Cell from, Cell to)
        {
            Debug.Log($"Moving object from {from.transform.position} to {to.transform.position}");
            to.square = from.square;
            to.square.gameObject.transform.position = to.transform.position;
            from.empty = true;
            to.empty = false;
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
                        current.MergeAfterReachingTarget = nextSquare;
                        currentPosition = nextPosition;
                        someMoved = true;
                    }
                }

                break;
            }
            nextPosition += diraction;
        }

        current.TargetPosition = currentPosition;
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

    void Merge(Square from, Square to)
    {
        
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
        SpawnRandomSquare();
        SpawnRandomSquare();
    }

    private void SpawnRandomSquare()
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
        square.TargetPosition = position;
        Set(position,square);    
    }
    // void SpawnRandomCell()
        // {
        //     if (!CanSpawn)
        //     {
        //         return;
        //     }
        //     System.Random r = new System.Random();
        //     IEnumerable<Cell> emptyCells = cells.Where(x => x.empty);
        //     int count = emptyCells.Count();
        //     if (count==0)
        //     {
        //         CanSpawn = false;
        //         return;
        //     }
        //     Cell randomCell = emptyCells.ElementAt(r.Next(emptyCells.Count()-1));
        //     randomCell.Spawn();
}
