using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.Analytics;

public class Square : MonoBehaviour
{
    public TextMeshPro text;
    public Color[] colors;
    public bool CanMerge = true;
    public int Weight
    {
        get => weight+MergeAfterReachingTarget.Count;
        set
        {
            weight = value;
            text.text = weight > 0 ? Mathf.Pow(2, weight).ToString() : "";
            
        }
    }
    public float speed = 5;
    private int weight;
    public Queue<Vector2> TargetPosition  = new Queue<Vector2>();
    public Queue<Square> MergeAfterReachingTarget = new Queue<Square>();

    public static Square Merge(Square from, Square to)
    {
        from.Weight++;
        Destroy(to.gameObject);
        from.SetColor();
        return from;
    }
    
    void SetColor()
    {
        var sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.color = weight>colors.Length-1 ? colors.Last() : colors[weight];
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        SetColor();
        for (float i = 0; i < 1; i+=Time.deltaTime/0.3f)
        {
            float fade = Mathf.Lerp(0, 1, i);
            fade = 1 - (1 - fade) * (1 - fade) * (1 - fade);
            transform.localScale = new Vector3(fade,fade,fade);
            yield return null;
        }
        
        transform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.Instance.GameOnPause)
        {
            MoveSquare();
        }
    }

    public SquareInformation Dump()
    {
        Vector2 pos = transform.localPosition;
        
        if (TargetPosition.Count!=0)
        {
            pos = TargetPosition.Last();
        }

        int w = this.Weight;
        
        return new SquareInformation(pos,w);
    }

    public void Load(SquareInformation square)
    {
        transform.localPosition = square.Position;
        Weight = square.Weight;
        TargetPosition.Clear();
        foreach (Square sq in MergeAfterReachingTarget)
        {
            Destroy(sq);
        }
        MergeAfterReachingTarget.Clear();
    }

    private void MoveSquare()
    {
        if (TargetPosition.Count == 0) return;
        Vector2 target = TargetPosition.Peek();
        Vector3 nextPosition = Vector2.MoveTowards(transform.localPosition, target, Time.deltaTime * speed);
        transform.localPosition = nextPosition;

        if ((Vector2) transform.localPosition != target) return;
        transform.localPosition = target;
        TargetPosition.Dequeue();
        
        if (MergeAfterReachingTarget.Count == 0) return;
        Square sq = MergeAfterReachingTarget.Peek();
        if (!((Vector2)sq.transform.localPosition==target)) return;
        MergeAfterReachingTarget.Dequeue();
        Merge(this, sq);
    }

    public static bool Compatible(Square a, Square b)
    {
        if (a != null && b!=null)
        {
            return a.Weight == b.Weight;
        }
        return false;
    }
}
