using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool empty = true;
    public bool HasMerged = false;

    public int Value;
    public Square square = null;
    
    void Start()
    {
    }

    public void Spawn()
    {
        square = Instantiate(square, transform.position,quaternion.identity);
        System.Random r = new System.Random();
        int number = r.Next(100) > 95 ? 2 : 1;
        square.Weight = number;
        empty = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
