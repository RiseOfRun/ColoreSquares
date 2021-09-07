using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Diagnostics;
using Random = UnityEngine.Random;

public class GameField : MonoBehaviour
{
    public float Padding = 2;
    public GameObject TilePrefab;
    public Cell CellPrefab;
    public bool CanSpawn = true;
    
    public int rows = 4;
    public int cols = 4;
    private float cellSize;
    private float space;
    Cell[,] field;
    private List<Cell> cells = new List<Cell>();
    
    // Start is called before the first frame update
    void Start()
    {
        BuildGrid();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SpawnRandomCell();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnInput(Vector2.down);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnInput(Vector2.left);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnInput(Vector2.right);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnInput(Vector2.up);
        }
    }

    private void OnInput(Vector2 diraction)
    {
        int start = diraction.x > 0 || diraction.y < 0 ? rows-1 : 0;
        int dir = diraction.x > 0 || diraction.y < 0 ? -1 : 1;
        
        for (int i = start+dir; i >= 0 && i < rows; i+=dir)
        {
            for (int j = 0; j < rows; j++)
            {
                bool row = diraction.x == 0;
                Cell currentCell = !row ? field[j,i] : field[i,j];
                if (currentCell.empty)
                {
                    continue;
                }
                
                Cell nextCell = null;
                for (int k = i-dir; k >=0 && k<rows; k-=dir)
                {
                    row = diraction.x == 0;
                    Cell tmp = row ? field[k,j] : field[j,k];
                    if (tmp.empty)
                    {
                        nextCell = tmp;
                        continue;
                    }
                    if (!tmp.HasMerged)
                    {
                        break;
                        nextCell = tmp;
                    }
                }
                if (nextCell != null)
                { 
                    MoveTo(currentCell,nextCell);
                }
            }
        }
    }

    void MoveTo(Cell from, Cell to)
    {
        Debug.Log($"Moving object from {from.transform.position} to {to.transform.position}");
        to.square = from.square;
        to.square.gameObject.transform.position = to.transform.position;
        from.empty = true;
        to.empty = false;
    }
    

    void BuildGrid()
    {
        cellSize = CellPrefab.gameObject.GetComponentInChildren<SpriteRenderer>().bounds.size.x;
        float fieldSize = GetComponentInChildren<SpriteRenderer>().bounds.size.x;
        space = (fieldSize - Padding * 2 - cellSize * cols) / (cols - 1);
        Padding += cellSize / 2f;
        field = new Cell[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector3 position = new Vector3(
                    Padding + space * (j) + cellSize * j,
                    -Padding - space * (i) - cellSize * i,
                    0
                );
                field[i, j] = Instantiate(CellPrefab, transform);
                field[i, j].transform.localPosition = position;
                cells.Add(field[i, j]);
            }
        }
    }

    void SpawnRandomCell()
    {
        if (!CanSpawn)
        {
            return;
        }
        System.Random r = new System.Random();
        IEnumerable<Cell> emptyCells = cells.Where(x => x.empty);
        int count = emptyCells.Count();
        if (count==0)
        {
            CanSpawn = false;
            return;
        }
        Cell randomCell = emptyCells.ElementAt(r.Next(emptyCells.Count()-1));
        randomCell.Spawn();
    }
}
