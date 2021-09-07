using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;

public class Square : MonoBehaviour
{
    public TextMeshPro text;

    public int Weight
    {
        get => weight;
        set
        {
            weight = value;
            text.text = weight > 0 ? Mathf.Pow(2, weight).ToString() : "";
        }
    }

    private int weight;
    
    
    

    public void Merge()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
